using StateMachine.src;
using StateMachine.src.Entities;
using StateMachine.src.Entities.Enemy.EGeneric;

var player = new Player();
player.transform.SetPosition(20, 20);

var wolf1 = new EGeneric("wolf1", player);
wolf1.transform.SetPosition(1, 1);

var wolf2 = new EGeneric("wolf2", player);
wolf2.transform.SetPosition(20, 1);

var wolf3 = new EGeneric("wolf3", player);
wolf3.transform.SetPosition(1, 20);

IEnumerable<EGeneric> enemies = [
    wolf1,
    wolf2,
    wolf3,
];


GameWindow window = new(player, enemies);


