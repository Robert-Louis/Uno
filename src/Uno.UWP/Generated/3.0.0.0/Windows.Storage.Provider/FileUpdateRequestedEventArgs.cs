#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Storage.Provider
{
	#if __ANDROID__ || __IOS__ || NET46 || __WASM__
	[global::Uno.NotImplemented]
	#endif
	public  partial class FileUpdateRequestedEventArgs 
	{
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		[global::Uno.NotImplemented]
		public  global::Windows.Storage.Provider.FileUpdateRequest Request
		{
			get
			{
				throw new global::System.NotImplementedException("The member FileUpdateRequest FileUpdateRequestedEventArgs.Request is not implemented in Uno.");
			}
		}
		#endif
		// Forced skipping of method Windows.Storage.Provider.FileUpdateRequestedEventArgs.Request.get
	}
}
