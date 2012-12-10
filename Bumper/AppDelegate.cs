using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Bump3Binding;

namespace Bumper
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);

			var rootViewController = new UIViewController();

			var rootView = new UIView();
			rootView.BackgroundColor = UIColor.White;

			rootViewController.View = rootView;

			window.RootViewController = rootViewController;
			
			// make the window visible
			window.MakeKeyAndVisible ();

			ConfigureBump();
			
			return true;
		}

		/*
		 * Bump Flow:
		 * 
		 * - Configure
		 * - Bump
		 * - Match
		 * - Confirm match
		 * - Send Data
		 * - Receive Data
		 */

		public void ConfigureBump ()
		{
			BumpClient.Configure("your_app_key", UIDevice.CurrentDevice.Name);

			BumpClient.SharedClient.SetMatchBlock(delegate(ulong channelID){
				Console.WriteLine("Matched with user: {0}", BumpClient.SharedClient.UserIDForChannel(channelID));

				BumpClient.SharedClient.ConfirmMatch(true, channelID);
			});

			BumpClient.SharedClient.SetChannelConfirmedBlock(delegate(ulong channelID) {
				Console.WriteLine("Channel with {0} confirmed.", BumpClient.SharedClient.UserIDForChannel(channelID));

				BumpClient.SharedClient.SendData(NSData.FromString("Hello World!"), channelID);
			});

			BumpClient.SharedClient.SetDataReceivedBlock(delegate(ulong channelID, NSData data) {
				Console.WriteLine("Data received from {0}: {1}", BumpClient.SharedClient.UserIDForChannel(channelID), data.ToString());
			});

			BumpClient.SharedClient.SetConnectionStateChangedBlock(delegate(bool connected) {
				if (connected) {
					Console.WriteLine("Bump connected...");
				} else {
					Console.WriteLine("Bump disconnected...");
				}
			});

			BumpClient.SharedClient.SetBumpEventBlock(delegate(BumpEvent evt) {
				switch(evt) {
				case BumpEvent.BUMP_EVENT_BUMP:
					Console.WriteLine("Bump detected.");
					break;
				case BumpEvent.BUMP_EVENT_NO_MATCH:
					Console.WriteLine("No match.");
					break;
				}
			});
		}
	}

	//Dirty hack to make autocompletion work
	/*
	enum BumpEvent
	{
		BUMP_EVENT_BUMP = 0,
		BUMP_EVENT_NO_MATCH = 1
	}

	class BumpClient {
		delegate void BumpEventBlock(BumpEvent evt);
		delegate void BumpMatchBlock(ulong proposedChannelID);
		delegate void BumpChannelConfirmedBlock(ulong channelID);
		delegate void BumpDataReceivedBlock(ulong channel,NSData data);
		delegate void BumpConnectionStateChangedBlock(bool connectedToBumpServer);

		public static BumpClient SharedClient { get; set; }

		public void Configure(string APIKey, string userID) {}
		public void SetBumpEventBlock(BumpEventBlock bumpEventBlock) {}
		public void SetMatchBlock(BumpMatchBlock matchBlock) {}
		public void SetChannelConfirmedBlock(BumpChannelConfirmedBlock confirmedBlock) {}
		public void SetDataReceivedBlock(BumpDataReceivedBlock dataReceivedBlock) {}
		public void SetConnectionStateChangedBlock(BumpConnectionStateChangedBlock connectionBlock) {}
		public void ConfirmMatch(bool confirmed, ulong proposedChannelID) {}
		public void SendData(NSData data, ulong channelID) {}
		public NSString UserIDForChannel(ulong channelID) { return null; }

		public bool Bumpable { get; set; }

		public void SimulateBump() {}
	}
	*/
}

