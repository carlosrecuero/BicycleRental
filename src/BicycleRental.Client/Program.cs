// See https://aka.ms/new-console-template for more information
using Dapr.Actors.Client;
using Dapr.Actors;
using BicycleRental.Interfaces.Cards;


const string WalletActorType = "WalletActor";
const string BicycleActorType = "BicycleActor";

Console.WriteLine("Running simulation. Press any key to start.............");
Console.ReadLine();


var walletActor1 = ActorProxy.Create<IWalletActor>(new ActorId("000001"), WalletActorType);
var walletActor2 = ActorProxy.Create<IWalletActor>(new ActorId("000002"), WalletActorType);

await walletActor1.Reset();
await walletActor2.Reset();
await walletActor1.Deposit(1000);
await walletActor2.Deposit(1000);

var currentBalanceActor1 = await walletActor1.GetBalance();
Console.WriteLine($"Card 1: balance is {currentBalanceActor1}");

var currentBalanceActor2 = await walletActor2.GetBalance();
Console.WriteLine($"Card 2: balance is {currentBalanceActor2}");


//!! los métodos de los actores deben devolver  Task

var bicycleActorA = ActorProxy.Create<IBicycleActor>(new ActorId("A"), BicycleActorType);
var bicycleActorB = ActorProxy.Create<IBicycleActor>(new ActorId("B"), BicycleActorType);

await bicycleActorA.Reset();
await bicycleActorB.Reset();

await bicycleActorA.StartRenting("000001");
//await Task.Delay(1000);
await bicycleActorB.StartRenting("000002");
//await Task.Delay(2000);
await bicycleActorB.FinishRenting();
//await Task.Delay(2500);
await bicycleActorA.FinishRenting();

currentBalanceActor1 = await walletActor1.GetBalance();
Console.WriteLine($"Card 1: balance is {currentBalanceActor1}");

currentBalanceActor2 = await walletActor2.GetBalance();
Console.WriteLine($"Card 2: balance is {currentBalanceActor2}");

//añadir kilometraje

Console.WriteLine("Simulation finished");