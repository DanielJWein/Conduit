# Usage
There are many different ways to use Conduit depending on what you want to achieve.

*Just looking to run the apps? See [APP_USAGE.md](APP_USAGE.md).*

*Looking for a class you can embed for easy streaming? Check out the Turnkey section below!*

> [!NOTE]
> Make sure to `.Dispose()` all of the following when you're done with them!

The decoder and encoder are provided in `ConduitEncoder` and `ConduitDecoder`. They provide the most basic encoding and decoding processes. They can be used as below:

### ConduitEncoder
```cs
ConduitEncoder encoder = null;

void Setup() {
    encoder = new ConduitEncoder();
    encoder.OnFrameAvailable += encoderFrameAvailable;
}

//PCM data format is controlled by ConduitCodecBase.WaveFmt.
//Change it before instantiating a ConduitEncoder or
//ConduitDecoder to change the PCM format used.
//It defaults to 48Khz 2 channel 16-bit PCM
void AddSamples(byte[] PCMData) {
    encoder.AddSamples(PCMData, 0, PCMData.Length);
}

void encoderFrameAvailable(object? sender, EventArgs frame) {
    ConduitCodecFrame frame = encoder.GetFrame();

    //Send the frame somehow.
}
```

### ConduitDecoder 
```cs
ConduitDecoder decoder = null;

void Setup() {
    decoder = new ConduitDecoder();
    decoder.OnDecodedFrame += decoderFrameDecoded;
}

//Get the Frame from somewhere else
void WeGotSomeDataFromSomewhere(ConduitCodecFrame frame) {
    decoder.DecodeFrame(frame);
}

void decoderFrameDecoded(object? sender, EventArgs args) {
    //The PCM data is in decoder.Buffer, which is an NAudio.IWaveProvider. You can play this into any NAudio class that takes one (like WaveOutEvent or WaveFileWriter).

    //See ConduitTurnkeyClient for a usage example.
}
```

<hr style="border-bottom: 2px solid #88F">


The ConduitClient and ConduitServer classes abstract away much of the socket communication. They're ideal for apps that want to automate the transfer of packets, but still want to control the actual audio pipeline (both input and output of data).

### ConduitClient
```cs
ConduitClient client = null;
bool killThread = false;

void Setup() {
    //Default port is 32662
    client = new ConduitClient("some.conduit.server");

    client.Connect();
}

void ThreadGetAudio() {
    while(!killThread) {
        if(!client.Connected) {
            killThread = true;
            break;
        }
        else {
            try {
                //Process control packets (such as disconnect)
                _ = client.ProcessControlPacket();
            }
            //The server disconnected.
            catch (DisconnectedException) {
                killThread = true;
                client.Dispose();
                return;
            }
            //Get a frame from the client (may have 0 frames ready)
            ConduitCodecFrame? frame = client.GetFrame();
            if (frame == null) {
                continue;
            }
            else {
                //Do something with frame. Play it, 
                //record it, etc.
            }
        }
    }
}

void Disconnect() {
    client.Disconnect();
}
```

### ConduitServer

```cs
ConduitServer server = null;

//Starts the server. Call StartListening to get new clients.
void Setup() {
    //Bind to {Any} on 32662
    server = new ConduitServer();
}

//Start accepting new clients
void StartListening() {
    server.StartListening();
}

//Stop accepting new clients
void StopListening() {
    server.StopListening();
}

//Sends a data frame to all clients
void SendFrameToClients(ConduitCodecFrame data) {
    server.Send(data);
}

//Check out ConduitControlPacket.cs for defined controls
//and ConduitControlProtocol.txt for more info.
void SendControlPacketToClients(byte[] controlPacket) {
    sever.Send(controlPacket);
}

//Closes the server
void Close() {
    server.Close();
}
```

> [!NOTE]
> The server will not send empty frames. If you have long stretches of complete silence in your audio, the server will truncate these.
>
> To remedy this, you can set `server.SendEmptyPackets` to `true`.

<hr style="border-bottom: 2px solid #88F">

And finally, for the turbo-lazy or turbo-busy, we have the Turnkey set, which does nearly everything for you. The server will take in data from an `NAudio.IWaveProvider` and automatically send it to clients. Turnkey clients will automatically play the data to the default audio device.

### ConduitTurnkeyClient
```cs
ConduitTurnkeyClient client = null;

void Setup() {
    //Set up the auto-update thread and open the
    //default output device for automatic output.
    client = new ConduitTurnkeyClient("some.conduit.server");
    //Actually connect to the server.
    client.Connect();
}

void Disconnect() {
    client.Disconnect();
}
```

### ConduitTurnkeyServer
```cs
ConduitTurnkeyServer server = null;

void Setup(IWaveProvider waveProvider) {
    //Automatically set up a thread to read data from
    //the waveProvider and send that data to clients.
    server = new ConduitTurnkeyServer(waveProvider);
}

//Start accepting new clients
void StartListening() {
    server.StartListening();
}

//Stop accepting new clients
void StopListening() {
    server.StopListening();
}

//Sends a data frame to all clients
void SendFrameToClients(ConduitCodecFrame data) {
    server.Send(data);
}

//Check out ConduitControlPacket.cs for defined controls
//and ConduitControlProtocol.txt for more info.
void SendControlPacketToClients(byte[] controlPacket) {
    sever.Send(controlPacket);
}

//Closes the server
void Close() {
    server.Close();
}
```

> [!TIP]
> If you use an `NAudio.BufferedWaveProvider` with `ReadFully` set to `true`, you can easily create a server that will have an indefinite stream and you can just put samples in the buffer to send them.