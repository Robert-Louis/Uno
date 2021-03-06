#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Data.Xml.Dom
{
	#if __ANDROID__ || __IOS__ || NET46 || __WASM__
	[global::Uno.NotImplemented]
	#endif
	public  partial interface IXmlCharacterData : global::Windows.Data.Xml.Dom.IXmlNode,global::Windows.Data.Xml.Dom.IXmlNodeSelector,global::Windows.Data.Xml.Dom.IXmlNodeSerializer
	{
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		string Data
		{
			get;
			set;
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		uint Length
		{
			get;
		}
		#endif
		// Forced skipping of method Windows.Data.Xml.Dom.IXmlCharacterData.Data.get
		// Forced skipping of method Windows.Data.Xml.Dom.IXmlCharacterData.Data.set
		// Forced skipping of method Windows.Data.Xml.Dom.IXmlCharacterData.Length.get
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		string SubstringData( uint offset,  uint count);
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		void AppendData( string data);
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		void InsertData( uint offset,  string data);
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		void DeleteData( uint offset,  uint count);
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		void ReplaceData( uint offset,  uint count,  string data);
		#endif
	}
}
