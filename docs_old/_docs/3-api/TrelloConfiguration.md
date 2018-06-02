---
title: TrelloConfiguration
category: API
order: 256
---

Exposes a set of run-time options for Manatee.Trello.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- Object
- TrelloConfiguration

## Properties

### static [ICache](../ICache#icache) Cache { get; set; }

Provides a cache to manage all Trello objects.

### static TimeSpan ChangeSubmissionTime { get; set; }

Specifies a length of time an object holds changes before it submits them. The timer is reset with every change. Default is 100 ms.

#### Remarks

Setting a value of 0 ms will result in instant upload of changes, dramatically increasing call volume and slowing performance.

### static TimeSpan DelayBetweenRetries { get; set; }

Specifies a delay between retry attempts.

### static [IDeserializer](../IDeserializer#ideserializer) Deserializer { get; set; }

Specifies the deserializer for the REST client.

### static [IJsonFactory](../IJsonFactory#ijsonfactory) JsonFactory { get; set; }

Provides a factory which is used to create instances of JSON objects.

### static [ILog](../ILog#ilog) Log { get; set; }

Provides logging for Manatee.Trello. The default log writes to the Console window.

### static int MaxRetryCount { get; set; }

Specifies a maximum number of retries allowed before an error is thrown.

### static TimeSpan RefreshThrottle { get; set; }

Specifies a length of time during which a single entity can only be refreshed once. Default is 5 seconds.

#### Remarks

Setting a value of 0 will result in immediately consecutive or parallel calls both going through to Trello.

### static bool RemoveDeletedItemsFromCache { get; set; }

Specifies whether deleted items should be removed from the cache. The default is true.

### static [IRestClientProvider](../IRestClientProvider#irestclientprovider) RestClientProvider { get; set; }

Specifies the REST client provider.

### static Func&lt;[IRestResponse](../IRestResponse#irestresponse), int, bool&gt; RetryPredicate { get; set; }

Specifies a predicate to execute to determine if a retry should be attempted. The default simply uses Manatee.Trello.TrelloConfiguration.MaxRetryCount and Manatee.Trello.TrelloConfiguration.DelayBetweenRetries.

#### Remarks

Parameters:

- Manatee.Trello.Rest.IRestResponse - The response object from the REST provider. Will need to be cast to the appropriate type.
- System.Int32 - The number of retries attempted.

Return value:

- System.Boolean - True if the call should be retried; false otherwise.

### static IList&lt;HttpStatusCode&gt; RetryStatusCodes { get; }

Specifies which HTTP response status codes should trigger an automatic retry.

### static [ISerializer](../ISerializer#iserializer) Serializer { get; set; }

Specifies the serializer for the REST client.

### static bool ThrowOnTrelloError { get; set; }

Specifies whether the service should throw an exception when an error is received from Trello. Default is true.

## Methods

### static void RegisterPowerUp(string id, Func&lt;IJsonPowerUp, TrelloAuthorization, IPowerUp&gt; factory)

Registers a new power-up implementation.

**Parameter:** id

The Trello ID of the power-up.

**Parameter:** factory

A factory function that creates instances of the power-up implementation.
