#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Devices.Sensors
{
	#if __ANDROID__ || __IOS__ || NET46 || __WASM__
	#if __ANDROID__ || __IOS__ || NET46 || __WASM__
	[global::Uno.NotImplemented]
	#endif
	public   enum AccelerometerReadingType 
	{
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		Standard,
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		Linear,
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		Gravity,
		#endif
	}
	#endif
}