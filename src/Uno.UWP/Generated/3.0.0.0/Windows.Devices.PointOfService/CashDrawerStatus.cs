#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Devices.PointOfService
{
	#if __ANDROID__ || __IOS__ || NET46 || __WASM__
	[global::Uno.NotImplemented]
	#endif
	public  partial class CashDrawerStatus 
	{
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		[global::Uno.NotImplemented]
		public  uint ExtendedStatus
		{
			get
			{
				throw new global::System.NotImplementedException("The member uint CashDrawerStatus.ExtendedStatus is not implemented in Uno.");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET46 || __WASM__
		[global::Uno.NotImplemented]
		public  global::Windows.Devices.PointOfService.CashDrawerStatusKind StatusKind
		{
			get
			{
				throw new global::System.NotImplementedException("The member CashDrawerStatusKind CashDrawerStatus.StatusKind is not implemented in Uno.");
			}
		}
		#endif
		// Forced skipping of method Windows.Devices.PointOfService.CashDrawerStatus.StatusKind.get
		// Forced skipping of method Windows.Devices.PointOfService.CashDrawerStatus.ExtendedStatus.get
	}
}
