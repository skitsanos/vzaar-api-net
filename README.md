vzaar API .NET client
---
Lighter and faster vzaar API client for .NET and Mono developers

---

>vzaar is the go to video hosting platform for business. Affordable, customizable and secure. Leverage the power of online video and enable commerce with vzaar.

----

###Examples


To start, make sure you have included com.vzaar.api.dll into your Project References.

```csharp
using com.vzaar.api;
```
Now you can create your vzaar client instance:

```csharp
var api = new Vzaar("VZAAR_USERNAME", "VZAAR_TOKEN");
```
In order to use vzaar API, you need to have a valid user name and API token that you can get from your vzaar dashboard at [http://vzaar.com/settings/third_party](http://vzaar.com/settings/third_party).

The very next thing you would want to do is to check if your account actually works and operational and you can do it by simple calling _whoAmI()_:

```csharp
Debug.WriteLine(api.whoAmI());
```

If it returns you your vzaar username, - we are good to go.

####User Details

>This API call returns the user's public details along with it's relevant metadata. It also contains vzaar Account ID that you can use in _api.getAccountDetails_ call.

```csharp
var details = api.getUserDetails(VZAAR_USERNAME);
```

Where _VZAAR_USERNAME_ is the vzaar username. Result of this call will be an object of [UserDetails](com.vzaar.api/UserDetails.cs) type.

####Account Details

>This API call returns the details and rights for each vzaar subscription account type along with it's relevant metadata. This will show the details of the packages available here: [http://vzaar.com/pricing](http://vzaar.com/pricing)

```csharp
var details = api.getAccountDetails(VZAAR_ACCOUNT_ID);
```

Where _VZAAR_ACCOUNT_ID_ is the unique account id assigned by vzaar.

Result of this call will be an object of [AccountDetails](com.vzaar.api/AccountDetails.cs) type.

####Video List
>This API call returns a list of the user's active videos along with it's relevant metadata. 20 videos are returned by default but this is customizable.

```csharp
var query = new VideoListQuery
{
	count = 10,
	page = 1
};

var list = api.getVideoList(query);
```

####Video Details

```csharp
api.getVideoDetails(1561281);
```
####Upload Signature

>In some cases you might need to not perform actual uploading from API but to use some third-party uploaders, like S3_Upload widget, or any other, so you would need to get only upload signature for it, so now you can have it as UploadSignature object, as XML string, as XmlDocument or as JSON string:

```csharp
var jsonStringSignature = api.getUploadSignature().toJson();
```
