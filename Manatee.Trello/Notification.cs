﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Internal.Synchronization;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents a notification.
	/// </summary>
	public class Notification : INotification, IMergeJson<IJsonNotification>, IBatchRefresh, IHandleSynchronization
	{
		/// <summary>
		/// Enumerates the data which can be pulled for notifications.
		/// </summary>
		[Flags]
		public enum Fields
		{
			/// <summary>
			/// Indicates the Creator property should be populated.
			/// </summary>
			[Display(Description="idMemberCreator")]
			Creator = 1,
			/// <summary>
			/// Indicates the Data property should be populated.
			/// </summary>
			[Display(Description="data")]
			Data = 1 << 1,
			/// <summary>
			/// Indicates the IsUnread property should be populated.
			/// </summary>
			[Display(Description="unread")]
			IsUnread = 1 << 2,
			/// <summary>
			/// Indicates the Type property should be populated.
			/// </summary>
			[Display(Description="type")]
			Type = 1 << 3,
			/// <summary>
			/// Indicates the Date property should be populated.
			/// </summary>
			[Display(Description="date")]
			Date = 1 << 4
		}

		private static readonly Dictionary<NotificationType, Func<Notification, string>> _stringDefinitions;

		private readonly Field<Member> _creator;
		private readonly Field<DateTime?> _date;
		private readonly Field<bool?> _isUnread;
		private readonly Field<NotificationType?> _type;
		private readonly NotificationContext _context;
		private DateTime? _creation;
		private static Fields _downloadedFields;

		/// <summary>
		/// Specifies which fields should be downloaded.
		/// </summary>
		public static Fields DownloadedFields
		{
			get { return _downloadedFields; }
			set
			{
				_downloadedFields = value;
				NotificationContext.UpdateParameters();
			}
		}

		/// <summary>
		/// Gets the creation date of the notification.
		/// </summary>
		public DateTime CreationDate
		{
			get
			{
				if (_creation == null)
					_creation = Id.ExtractCreationDate();
				return _creation.Value;
			}
		}
		/// <summary>
		/// Gets the member who performed the action which created the notification.
		/// </summary>
		public IMember Creator => _creator.Value;
		/// <summary>
		/// Gets any data associated.
		/// </summary>
		public INotificationData Data { get; }
		/// <summary>
		/// Gets the date and time at which the notification was issued.
		/// </summary>
		public DateTime? Date => _date.Value;
		/// <summary>
		/// Gets the notification's ID.
		/// </summary>
		public string Id { get; private set; }
		/// <summary>
		/// Gets or sets whether the notification has been read.
		/// </summary>
		public bool? IsUnread
		{
			get { return _isUnread.Value; }
			set { _isUnread.Value = value; }
		}
		/// <summary>
		/// Gets the type of notification.
		/// </summary>
		public NotificationType? Type => _type.Value;

		internal IJsonNotification Json
		{
			get { return _context.Data; }
			set { _context.Merge(value); }
		}
		TrelloAuthorization IBatchRefresh.Auth => _context.Auth;

		/// <summary>
		/// Raised when data on the notification is updated.
		/// </summary>
		public event Action<INotification, IEnumerable<string>> Updated;

		static Notification()
		{
			_stringDefinitions = new Dictionary<NotificationType, Func<Notification, string>>
				{
					{NotificationType.AddedAttachmentToCard, n => $"{n.Creator} attached {n.Data.Attachment} to card {n.Data.Card}."},
					{NotificationType.AddedToBoard, n => $"{n.Creator} added you to board {n.Data.Board}."},
					{NotificationType.AddedToCard, n => $"{n.Creator} assigned you to card {n.Data.Card}."},
					{NotificationType.AddedToOrganization, n => $"{n.Creator} added member {n.Data.Member} to organization {n.Data.Organization}."},
					{NotificationType.AddedMemberToCard, n => $"{n.Creator} assigned member {n.Data.Member} to card {n.Data.Card}."},
					{NotificationType.AddAdminToBoard, n => $"{n.Creator} added member {n.Data.Member} to board {n.Data.Board} as an admin."},
					{NotificationType.AddAdminToOrganization, n => $"{n.Creator} added member {n.Data.Member} to organization {n.Data.Organization} as an admin."},
					{NotificationType.ChangeCard, n => $"{n.Creator} changed card {n.Data.Card}."},
					{NotificationType.CloseBoard, n => $"{n.Creator} closed board {n.Data.Board}."},
					{NotificationType.CommentCard, n => $"{n.Creator} commented on card #{n.Data.Card}: '{n.Data.Text}'."},
					{NotificationType.CreatedCard, n => $"{n.Creator} created card {n.Data.Card} on board {n.Data.Board}."},
					{NotificationType.RemovedFromBoard, n => $"{n.Creator} removed member {n.Data.Member} from board {n.Data.Board}."},
					{NotificationType.RemovedFromCard, n => $"{n.Creator} removed you from card {n.Data.Card}."},
					{NotificationType.RemovedMemberFromCard, n => $"{n.Creator} removed member {n.Data.Member} from card {n.Data.Card}."},
					{NotificationType.RemovedFromOrganization, n => $"{n.Creator} removed member {n.Data.Member} from organization {n.Data.Organization}."},
					{NotificationType.MentionedOnCard, n => $"{n.Creator} mentioned you on card {n.Data.Card}: '{n.Data.Text}'."},
					{NotificationType.UpdateCheckItemStateOnCard, n => $"{n.Creator} updated checkItem {n.Data.CheckItem} on card {n.Data.Card}."},
					{NotificationType.MakeAdminOfBoard, n => $"{n.Creator} made member {n.Data.Member} an admin of board {n.Data.Board}."},
					{NotificationType.MakeAdminOfOrganization, n => $"{n.Creator} made member {n.Data.Member} an admin of organization {n.Data.Organization}."},
					{NotificationType.CardDueSoon, n => $"Card {n.Data.Card} is due soon."},
					{NotificationType.AddAttachmentToCard, n => $"{n.Creator} added an attachment to {n.Data.Card}."},
					{NotificationType.MemberJoinedTrello, n => $"{n.Data.Member} joined Trello!"},
				
					{NotificationType.ReactionAdded, n => $"{n.Data.Member} added a reaction to a comment in card {n.Data.Card}."},
					{NotificationType.ReactionRemoved, n => $"{n.Data.Member} removed a reaction from a comment in card {n.Data.Card}"},
					{NotificationType.ReopenBoard, n => $"{n.Data.Member} reopened board {n.Data.Board}"},
					{NotificationType.ButlerRuleFailedBoard, n => $"Card {n.Data.Board} had a butler rule fail" },
					{NotificationType.ButlerQuotaApproachingMember, n => $"Approaching Butler quota for member {n.Data.Member}" }
				};
			DownloadedFields = (Fields)Enum.GetValues(typeof(Fields)).Cast<int>().Sum();
		}
		/// <summary>
		/// Creates a new <see cref="Notification"/> object.
		/// </summary>
		/// <param name="id">The notification's ID.</param>
		/// <param name="auth">(Optional) Custom authorization parameters. When not provided,
		/// <see cref="TrelloAuthorization.Default"/> will be used.</param>
		public Notification(string id, TrelloAuthorization auth = null)
		{
			Id = id;
			_context = new NotificationContext(id, auth);
			_context.Synchronized.Add(this);

			_creator = new Field<Member>(_context, nameof(Creator));
			_date = new Field<DateTime?>(_context, nameof(Date));
			Data = new NotificationData(_context.NotificationDataContext);
			_isUnread = new Field<bool?>(_context, nameof(IsUnread));
			_type = new Field<NotificationType?>(_context, nameof(Type));

			if (auth != TrelloAuthorization.Null)
				TrelloConfiguration.Cache.Add(this);
		}
		internal Notification(IJsonNotification json, TrelloAuthorization auth)
			: this(json.Id, auth)
		{
			_context.Merge(json);
		}

		/// <summary>
		/// Refreshes the notification data.
		/// </summary>
		/// <param name="force">Indicates that the refresh should ignore the value in <see cref="TrelloConfiguration.RefreshThrottle"/> and make the call to the API.</param>
		/// <param name="ct">(Optional) A cancellation token for async processing.</param>
		public Task Refresh(bool force = false, CancellationToken ct = default)
		{
			return _context.Synchronize(force, ct);
		}

		void IMergeJson<IJsonNotification>.Merge(IJsonNotification json, bool overwrite)
		{
			_context.Merge(json, overwrite);
		}

		Endpoint IBatchRefresh.GetRefreshEndpoint()
		{
			return _context.GetRefreshEndpoint();
		}

		void IBatchRefresh.Apply(string content)
		{
			var json = TrelloConfiguration.Deserializer.Deserialize<IJsonNotification>(content);
			_context.Merge(json);
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return Type.HasValue ? _stringDefinitions[Type.Value](this) : "Notification type could not be determined.";
		}

		void IHandleSynchronization.HandleSynchronized(IEnumerable<string> properties)
		{
			Id = _context.Data.Id;
			var handler = Updated;
			handler?.Invoke(this, properties);
		}
	}
}