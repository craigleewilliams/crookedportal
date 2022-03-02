# crookedportal

This is a Windows Forms app written in C# which levarages the Shopify and Etsy APIs to link identical product listings over both platforms. 
It also maintains consistency of updates to products and their variants across both platforms.
This app adds records of matched products and their variants to a database, acces to which is provided by Entity Framework using LINQ
This is a precursor to a .NET Core web app which will use Shopify APIs webhooks and regular polling of the Etsy API from a hosted environment in order to update 
stock levels in real time based on orders made from either platform.
