---
title: BoardMembershipCollection
category: API
order: 19
---

A collection of board memberships.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- Object
- ReadOnlyCollection&lt;IBoardMembership&gt;
- ReadOnlyBoardMembershipCollection
- BoardMembershipCollection

## Methods

### Task&lt;[IBoardMembership](../IBoardMembership#iboardmembership)&gt; Add(IMember member, BoardMembershipType membership, CancellationToken ct = default(CancellationToken))

Adds a member to a board with specified privileges.

**Parameter:** member

The member to add.

**Parameter:** membership

The membership type.

**Parameter:** ct

(Optional) A cancellation token for async processing.

**Returns:** The Manatee.Trello.IBoardMembership generated by Trello.

### Task Remove(IMember member, CancellationToken ct = default(CancellationToken))

Removes a member from a board.

**Parameter:** member

The member to remove.

**Parameter:** ct

(Optional) A cancellation token for async processing.
