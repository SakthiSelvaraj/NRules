﻿using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class CollectionAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_NewInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1), new TestFact(2));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Add_NoFacts_AddedResultEmptyCollection()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact[0]);
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(0, aggregate.Count());
        }

        [Fact]
        public void Add_NewInstanceUniqueFacts_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1), new TestFact(2));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Add_NewInstanceDuplicateFacts_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1), new TestFact(1));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Add_OldInstanceNewFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(EmptyTuple(), AsFact(new TestFact(1)));

            //Act
            var facts = AsFact(new TestFact(2), new TestFact(3));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(3, aggregate.Count());
        }

        [Fact]
        public void Modify_ExistingFactsSameValues_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(EmptyTuple(), facts);

            //Act
            var result = target.Modify(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Modify_ExistingFactsDifferentValues_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(3);
            facts[1].Value = new TestFact(4);
            var result = target.Modify(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Modify_ExistingDuplicateFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(1));
            target.Add(EmptyTuple(), facts);

            //Act
            var result = target.Modify(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(EmptyTuple(), facts);

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))));
        }

        [Fact]
        public void Remove_ExistingFactsSameValues_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(3));
            target.Add(EmptyTuple(), facts);

            //Act
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(1, aggregate.Count());
            Assert.Equal(3, aggregate.ElementAt(0).Id);
        }

        [Fact]
        public void Remove_ExistingFactsDifferentValues_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(3));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(3);
            facts[1].Value = new TestFact(4);
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(1, aggregate.Count());
            Assert.Equal(3, aggregate.ElementAt(0).Id);
        }

        [Fact]
        public void Remove_ExistingDuplicateFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(2));
            target.Add(EmptyTuple(), facts);

            //Act
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>) result[0].Aggregate;
            Assert.Equal(1, aggregate.Count());
            Assert.Equal(2, aggregate.ElementAt(0).Id);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2)));

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))));
        }

        private CollectionAggregator<TestFact> CreateTarget()
        {
            return new CollectionAggregator<TestFact>();
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id)
            {
                Id = id;
            }

            public int Id { get; }

            public bool Equals(TestFact other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TestFact) obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }
    }
}