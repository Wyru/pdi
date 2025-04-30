using StateMachine.src;
using StateMachine.src.Entities;
using StateMachine.src.Entities.Enemy.EGeneric;

var player = new Player();
player.transform.SetPosition(14, 14);

var wolf1 = new EGeneric("wolf1");
wolf1.transform.SetPosition(1, 1);

var wolf2 = new EGeneric("wolf2");
wolf1.transform.SetPosition(10, 1);

var wolf3 = new EGeneric("wolf3");
wolf1.transform.SetPosition(1, 10);

IEnumerable<EGeneric> enemies = [
    wolf1,
    wolf2,
    wolf3,
];


GameWindow window = new();


