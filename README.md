Flipper
=======

Flipper is a small library for flipping features on and off in .NET projects.

It was inspired by the rails gem rollout. https://github.com/jamesgolick/rollout

It is available via a nuget package https://www.nuget.org/packages/Flipper

    Install-Package Flipper

Examples
========

Create an actuator

    var actuator = new Flipper.Actuator(new RedisClient());

Define a new feature

	var feature = actuator.Get("feature");

Allowing a percentage of users

	actuator.ActivatePercentage(feature, 10); //Allows 10% of users to use the feature

Creating a group

	actuator.DefineGroup("betaUsers", new List<string> () { "1", "2", "15" });

Assign a group to a feature

	actuator.ActivateGroup(feature, "betaUsers");

Assign a user to a feature

	actuator.ActivateUser(feature, "1");


Check if a feature is active

	actuator.IsActive(feature); //this returns true only if the percentage is 100

	actuator.IsActive(feature, "1"); //this returns true if the user is in the percentage, users, or a group


v 0.1.1
=======

- Renamed Flipper class to Actuator

v 0.1
=====

- Added test coverage of basic features


Roadmap
=======

- Document Flipper.cs and Feature.cs
- Add and example project showing use
- More Tests
- Add extendable percentage calculating
