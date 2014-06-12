using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Engine;
using Microsoft.AspNet.SignalR;

namespace Web
{
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instance =
            new Lazy<Broadcaster>(() => new Broadcaster());
        // We're going to broadcast to all clients a maximum of 25 times per second
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(40);
        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;

        public static Frame Model
        {
            get { return _model; }
            private set { _model = value; }
        }

        private bool _modelUpdated;
        private static Frame _model;

        public Broadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

            var walls = new List<WallTile>
                        {
                            // top                             // bottom                         // left                           // right
                            new WallTile(new Position(0, 0)),  new WallTile(new Position(0, 7)), new WallTile(new Position(0, 0)), new WallTile(new Position(7, 0)),
                            new WallTile(new Position(1, 0)),  new WallTile(new Position(1, 7)), new WallTile(new Position(0, 1)), new WallTile(new Position(7, 1)),
                            new WallTile(new Position(2, 0)),  new WallTile(new Position(2, 7)), new WallTile(new Position(0, 2)), new WallTile(new Position(7, 2)),
                            new WallTile(new Position(3, 0)),  new WallTile(new Position(3, 7)), new WallTile(new Position(0, 3)), new WallTile(new Position(7, 3)),
                            new WallTile(new Position(4, 0)),  new WallTile(new Position(4, 7)), new WallTile(new Position(0, 4)), new WallTile(new Position(7, 4)),
                            new WallTile(new Position(5, 0)),  new WallTile(new Position(5, 7)), new WallTile(new Position(0, 5)), new WallTile(new Position(7, 5)),
                            new WallTile(new Position(6, 0)),  new WallTile(new Position(6, 7)), new WallTile(new Position(0, 6)), new WallTile(new Position(7, 6)),
                            new WallTile(new Position(7, 0)),  new WallTile(new Position(7, 7)), new WallTile(new Position(0, 7)), new WallTile(new Position(7, 7))
                        };
            Model = new Frame(tiles: walls);

            _modelUpdated = true;

            // Start the broadcast loop
            _broadcastLoop = new Timer(
                BroadcastFrame,
                null,
                BroadcastInterval,
                BroadcastInterval);
        }
        public void BroadcastFrame(object state)
        {
            // No need to send anything if our model hasn't changed
            if (_modelUpdated)
            {
                // This is how we can access the Clients property 
                // in a static hub method or outside of the hub entirely
                _hubContext.Clients.All.updateFrame(Model);
                _modelUpdated = false;
            }
        }
        public void UpdateFrame(Frame clientModel)
        {
            Model = clientModel;
            _modelUpdated = true;
        }

        public static Broadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
    public class GameHub : Hub
    {
	    private Broadcaster _broadcaster;
	    public GameHub()
	        : this(Broadcaster.Instance)
	    {
	    }

        public GameHub(Broadcaster broadcaster)
	    {
	        _broadcaster = broadcaster;
	    }

        public void UpdateModel(Frame clientModel)
        {
            _broadcaster.UpdateFrame(clientModel);
        }

        public void Send(string message)
        {
            _broadcaster.UpdateFrame(
                Broadcaster.Model.MovePlayer(Context.ConnectionId, (Direction) Enum.Parse(typeof (Direction), message))
                );
        }


        public override Task OnConnected()
        {
            var x = Interlocked.Increment(ref _x);
            var random = new ThreadLocal<Random>(() => new Random(x)).Value;

            var player = new PacmanPlayer(
                id: Context.ConnectionId,
                name: x.ToString(),
                position: new Position(random.Next(1, 7), random.Next(1, 7)));
            
            _broadcaster.UpdateFrame(Broadcaster.Model.AddPlayer(player).AdvanceTick());

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            _broadcaster.UpdateFrame(Broadcaster.Model.RemovePlayer(Context.ConnectionId).AdvanceTick());

            return base.OnDisconnected();
        }

        private static int _x = 0;
    }
}