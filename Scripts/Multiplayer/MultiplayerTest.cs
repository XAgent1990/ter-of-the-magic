using Godot;
using System;
using System.Reflection;

namespace TeroftheMagic.Multiplayer;

public partial class MultiplayerTest : Control {

    public static MultiplayerTest Instance { get; private set; }

    // These signals can be connected to by a UI lobby scene or the game scene.
    [Signal]
    public delegate void PlayerConnectedEventHandler(int peerId, Godot.Collections.Dictionary<string, string> playerInfo);
    [Signal]
    public delegate void PlayerDisconnectedEventHandler(int peerId);
    [Signal]
    public delegate void ServerDisconnectedEventHandler();

    private const int Port = 1806;
    private const string DefaultServerIP = "95.208.19.246"; // IPv4 localhost
    private const int MaxConnections = 20;

    // This will contain player info for every player,
    // with the keys being each player's unique IDs.
    private Godot.Collections.Dictionary<long, Godot.Collections.Dictionary<string, string>> _players = new Godot.Collections.Dictionary<long, Godot.Collections.Dictionary<string, string>>();

    // This is the local player info. This should be modified locally
    // before the connection is made. It will be passed to every other peer.
    // For example, the value of "name" can be set to something the player
    // entered in a UI scene.
    private Godot.Collections.Dictionary<string, string> _playerInfo = new Godot.Collections.Dictionary<string, string>()
    {
        { "Name", "XAgent" },
    };

    private int _playersLoaded = 0;

    public override void _Ready()
    {
        Instance = this;
        Multiplayer.PeerConnected += OnPlayerConnected;
        Multiplayer.PeerDisconnected += OnPlayerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectOk;
        Multiplayer.ConnectionFailed += OnConnectionFail;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    public Error JoinGame(string address = "")
    {
        if (string.IsNullOrEmpty(address))
        {
            address = DefaultServerIP;
        }

        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, Port);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;
        return Error.Ok;
    }

    public Error CreateGame()
    {
        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(Port, MaxConnections);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;
        _players[1] = _playerInfo;
        EmitSignal(SignalName.PlayerConnected, 1, _playerInfo);
        return Error.Ok;
    }

    private void RemoveMultiplayerPeer()
    {
        Multiplayer.MultiplayerPeer = null;
        _players.Clear();
    }

    // When the server decides to start the game from a UI scene,
    // do Rpc(Lobby.MethodName.LoadGame, filePath);
    [Rpc(CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void LoadGame(string gameScenePath)
    {
        GetTree().ChangeSceneToFile(gameScenePath);
    }

    // Every peer will call this when they have loaded the game scene.
    [Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void PlayerLoaded()
    {
        if (Multiplayer.IsServer())
        {
            _playersLoaded += 1;
            if (_playersLoaded == _players.Count)
            {
                //GetNode<Game>("/root/Game").StartGame();
				GD.Print($"{Multiplayer.GetUniqueId()}: All Players Loaded");
                _playersLoaded = 0;
            }
        }
    }

    // When a peer connects, send them my player info.
    // This allows transfer of all desired data for each player, not only the unique ID.
    private void OnPlayerConnected(long id)
    {
		GD.Print($"{Multiplayer.GetUniqueId()}: Player with ID {id} connected");
        RpcId(id, MethodName.RegisterPlayer, _playerInfo);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RegisterPlayer(Godot.Collections.Dictionary<string, string> newPlayerInfo)
    {
        int newPlayerId = Multiplayer.GetRemoteSenderId();
        _players[newPlayerId] = newPlayerInfo;
        EmitSignal(SignalName.PlayerConnected, newPlayerId, newPlayerInfo);
    }

    private void OnPlayerDisconnected(long id)
    {
		GD.Print($"{Multiplayer.GetUniqueId()}: Player with ID {id} disconnected");
        _players.Remove(id);
        EmitSignal(SignalName.PlayerDisconnected, id);
    }

    private void OnConnectOk()
    {
		GD.Print($"{Multiplayer.GetUniqueId()}: Connection successful");
        int peerId = Multiplayer.GetUniqueId();
        _players[peerId] = _playerInfo;
        EmitSignal(SignalName.PlayerConnected, peerId, _playerInfo);
    }

    private void OnConnectionFail()
    {
		GD.Print($"{Multiplayer.GetUniqueId()}: Connection failed");
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
		GD.Print($"{Multiplayer.GetUniqueId()}: Disconnected from Server");
        Multiplayer.MultiplayerPeer = null;
        _players.Clear();
        EmitSignal(SignalName.ServerDisconnected);
    }
}