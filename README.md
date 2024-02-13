# Mullvad VPN Auto-Management for Compliance with Fortnite TOS

This utility ensures compliance with the Fortnite Terms of Service, which prohibit the use of VPNs while playing. It automatically manages your Mullvad VPN connection by disconnecting when Fortnite is running and reconnecting once it's closed.

## Features

- 🎮 **Fortnite Compliance**: Automatically disconnects Mullvad VPN when `FortniteClient-Win64-Shipping.exe` is detected.
- 🔄 **Auto Reconnect**: Re-establishes the VPN connection once Fortnite and associated services are no longer running.
- 🔒 **Privacy First**: Ensures you are always protected by your VPN while not in-game.
- 🚀 **Startup Integration**: Configured to start with Windows, running silently in the background.
- 📝 **Logging**: Maintains logs for VPN disconnections and reconnections for troubleshooting.

## Prerequisites

Before you begin, ensure you have the following prerequisites installed:
- Windows 10 or newer.
- Mullvad VPN Client with CLI - [Installation Guide](https://mullvad.net/en/help/how-use-mullvad-cli/)
- .NET Runtime (If the application is distributed as a self-contained executable, this is not required.)
