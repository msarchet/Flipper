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

  //Allows 10% of users to use the feature
	actuator.ActivatePercentage(feature, 10); 

Creating a group

	actuator.DefineGroup("betaUsers", new List<string> () { "1", "2", "15" });

Assign a group to a feature

	actuator.ActivateGroup(feature, "betaUsers");

Assign a user to a feature

	actuator.ActivateUser(feature, "1");

Check if a feature is active

  //True only if the percentage is 100
	actuator.IsActive(feature); 

  //Indicates if the user is in the percentage, users, or a group
	actuator.IsActive(feature, "1"); 

v 0.2.1
=======

- Added Custom Func<string, Feature, bool> instead of hardcoded parsing
  - Default is same as the old way 

v 0.1.1
=======

- Renamed Flipper class to Actuator

v 0.1
=====

- Added test coverage of basic features


Roadmap
=======

- Add and example project showing use
- Make percentages accurate in lower sample sizes
- More Tests
