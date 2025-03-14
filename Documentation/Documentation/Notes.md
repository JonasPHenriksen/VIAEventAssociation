TODO:

Use a contract for time, where we have a default value from system time
Rewrite the test to use this so it is not depended on that
Time should be depended outside therefore we make a contract to get it from the outside, better depended on test

Update the Concrete domain model

Fix all the tests

Notes about DDD

1. Domain Contract

   What it is: A domain contract is an agreement or specification that defines how different parts of the domain interact.

   Purpose: It ensures that domain objects adhere to specific rules and behaviors.

   Example: An interface like ISystemTime is a domain contract. It defines how the system time is provided to the domain layer without specifying the implementation.

   Key Point: Contracts decouple the domain logic from implementation details, making the system more flexible and testable.

2. Domain Service

   What it is: A domain service is a stateless object that encapsulates domain logic that doesn’t naturally fit within an entity or value object.

   Purpose: It handles operations that involve multiple domain objects or require external dependencies (e.g., system time, APIs).

   Example: A SystemTime service that provides the current time is a domain service. It’s used by domain objects like EventTimeRange to enforce business rules.

   Key Point: Domain services are part of the domain layer and represent behaviors that are central to the domain but don’t belong to a specific entity or value object.

3. Aggregate

   What it is: An aggregate is a cluster of related objects (entities and value objects) treated as a single unit for data changes.

   Purpose: It enforces consistency and invariants (business rules) within the domain.

   Example: An Order aggregate might include an Order entity and a collection of OrderItem value objects. The aggregate ensures that the total order amount is always valid.

   Key Point: Aggregates have a root entity (e.g., Order) that acts as the entry point for interacting with the aggregate.

4. Entity

   What it is: An entity is a domain object that has a unique identity and a lifecycle. Its identity remains consistent even if its attributes change.

   Purpose: It represents something that is tracked and persisted over time.

   Example: A User entity has a unique UserId. Even if the user’s name or email changes, it’s still the same user.

   Key Point: Entities are mutable and are often the root of aggregates.

5. Value Object

   What it is: A value object is a domain object that is defined by its attributes and has no conceptual identity.

   Purpose: It represents a descriptive aspect of the domain with no lifecycle.

   Example: An Address value object is defined by its street, city, and postal code. Two addresses with the same attributes are considered equal.

   Key Point: Value objects are immutable and are often used within entities or aggregates.


    Domain Contract: Defines how domain objects interact (e.g., interfaces).

    Domain Service: Handles domain logic that doesn’t fit in entities or value objects.
    Or require external dependencies 

    Aggregate: A consistency boundary for related objects, with a root entity.

    Entity: Has a unique identity and lifecycle.

    Value Object: Defined by its attributes, immutable, and has no identity.