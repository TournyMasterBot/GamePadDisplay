# GamePadDisplay [SINGLE COMPUTER SCENARIO]
An extremely basic gamepad display for OBS studio

# Instructions to use as OBS scene

- Open application
- Open OBS Studio
- Add -> Game Capture
- Name the scene
- Select 'Capture specific window'
- Select 'GamePadDisplay' from the list
- Allow Transparency
- Do not capture mouse
- Do not use anti cheat compatability hook

# GamePadObsMachine [MULTI COMPUTER SCENARIO -- SERVER]

- Allow application through firewall (Should prompt automatically, if not add TCP port 42420)
- Run application
- Open OBS Studio
- Add -> Game Capture
- Name the scene
- Select 'Capture specific window'
- Select 'GamePadDisplay' from the list
- Allow Transparency
- Do not capture mouse
- Do not use anti cheat compatability hook

* If you are seeing extended lag, you can update config.ini to lower the update rate value, if you are seeing flickering buttons, increase the value for the update rate. The update rate is the time in milliseconds input needs to be received from the client before resetting the gamepad to the default state. (This number should be within 30ms of the gaming machine config in most scenarios.)

# GamePadGameMachine [MULTI COMPUTER SCENARIO -- GAMING MACHINE]

- Update config.ini to use the server computers IP (IE: 192.168.1.6 would be the remote OBS machine)
- Ensure server application is running
- Run client application, Be sure to allow the application through the firewall (allow on port 42420 if windows does not prompt you.)
- Play your game

* By default, config.ini sends updates to the server every 250ms, if you want this to happen faster, you can lower the value. Please note that lowering the value may increase CPU usage which could cause degredation in your games. If you update the client update rate, please also update the server update rate to be within 30ms of your new client value. 