using System;
using System.Collections.Generic;
using System.IO;
using NFGraph.Net.Build;
using NFGraph.Net.Compressed;
using NFGraph.Net.Spec;
using Xunit;

namespace NFGraph.Net.Tests
{
    public class UnitTest1
    {
        private readonly int NUM_A_NODES;
        private readonly int NUM_B_NODES;

        private static readonly NFGraphSpec Spec = new NFGraphSpec(
            new NFNodeSpec("node-type-a",
                new NFPropertySpec("a-to-one-b-global", "node-type-b", NFPropertySpec.SINGLE | NFPropertySpec.GLOBAL),
                new NFPropertySpec("a-to-one-b-per-model", "node-type-b", NFPropertySpec.SINGLE | NFPropertySpec.MODEL_SPECIFIC)
                ),

            new NFNodeSpec("node-type-b",
                new NFPropertySpec("b-to-many-a-compact-global", "node-type-a", NFPropertySpec.MULTIPLE | NFPropertySpec.COMPACT | NFPropertySpec.GLOBAL),
                new NFPropertySpec("b-to-many-a-hashed-global", "node-type-a", NFPropertySpec.MULTIPLE | NFPropertySpec.HASH | NFPropertySpec.GLOBAL),
                new NFPropertySpec("b-to-many-a-compact-per-model", "node-type-a", NFPropertySpec.MULTIPLE | NFPropertySpec.COMPACT | NFPropertySpec.MODEL_SPECIFIC),
                new NFPropertySpec("b-to-many-a-hashed-per-model", "node-type-a", NFPropertySpec.MULTIPLE | NFPropertySpec.HASH | NFPropertySpec.MODEL_SPECIFIC)
                )
            );

        private readonly int _seed;

        private readonly NFBuildGraph _buildGraph;
        //private NFGraph _graph;

        public UnitTest1()
        {
            var rand = new Random();

            NUM_A_NODES = rand.Next(10000);
            NUM_B_NODES = rand.Next(10000);

            _seed = Environment.TickCount;
            rand = new Random(_seed);

            _buildGraph = new NFBuildGraph(Spec);
            _buildGraph.AddConnectionModel("model-1");
            _buildGraph.AddConnectionModel("model-2");

            for (int i = 0; i < NUM_A_NODES; i++)
            {
                if (rand.NextBool())
                    _buildGraph.AddConnection("node-type-a", i, "a-to-one-b-global", rand.Next(NUM_B_NODES));
                if (rand.NextBool())
                    _buildGraph.AddConnection("model-1", "node-type-a", i, "a-to-one-b-per-model", rand.Next(NUM_B_NODES));
                if (rand.NextBool())
                    _buildGraph.AddConnection("model-2", "node-type-a", i, "a-to-one-b-per-model", rand.Next(NUM_B_NODES));
            }

            for (int i = 0; i < NUM_B_NODES; i++)
            {
                AddMultipleRandomConnections(rand, _buildGraph, i, "global", "b-to-many-a-compact-global");
                AddMultipleRandomConnections(rand, _buildGraph, i, "global", "b-to-many-a-hashed-global");
                AddMultipleRandomConnections(rand, _buildGraph, i, "model-1", "b-to-many-a-compact-per-model");
                AddMultipleRandomConnections(rand, _buildGraph, i, "model-2", "b-to-many-a-compact-per-model");
                AddMultipleRandomConnections(rand, _buildGraph, i, "model-1", "b-to-many-a-hashed-per-model");
                AddMultipleRandomConnections(rand, _buildGraph, i, "model-2", "b-to-many-a-hashed-per-model");
            }
        }

        [Theory]
        [InlineData, InlineData, InlineData, InlineData, InlineData]
        [InlineData, InlineData, InlineData, InlineData, InlineData]
        [InlineData, InlineData, InlineData, InlineData, InlineData]
        public void RandomizedTest()
        {
            NFCompressedGraph compressedGraph = _buildGraph.Compress();

            var outputStream = new MemoryStream();
            compressedGraph.WriteTo(outputStream);

            var inputStream = new MemoryStream(outputStream.ToArray());
            var graph = NFCompressedGraph.ReadFrom(inputStream);

            var rand = new Random(_seed);

            for (int i = 0; i < NUM_A_NODES; i++)
            {
                int conn = graph.GetConnection("node-type-a", i, "a-to-one-b-global");
                int expected = rand.NextBool() ? rand.Next(NUM_B_NODES) : -1;
                Assert.Equal(expected, conn);

                conn = graph.GetConnection("model-1", "node-type-a", i, "a-to-one-b-per-model");
                expected = rand.NextBool() ? rand.Next(NUM_B_NODES) : -1;
                Assert.Equal(expected, conn);

                conn = graph.GetConnection("model-2", "node-type-a", i, "a-to-one-b-per-model");
                expected = rand.NextBool() ? rand.Next(NUM_B_NODES) : -1;
                Assert.Equal(expected, conn);
            }

            for (int i = 0; i < NUM_B_NODES; i++)
            {
                AssertMultipleConnections(graph, rand, "global", i, "b-to-many-a-compact-global");
                AssertMultipleConnections(graph, rand, "global", i, "b-to-many-a-hashed-global");
                AssertMultipleConnections(graph, rand, "model-1", i, "b-to-many-a-compact-per-model");
                AssertMultipleConnections(graph, rand, "model-2", i, "b-to-many-a-compact-per-model");
                AssertMultipleConnections(graph, rand, "model-1", i, "b-to-many-a-hashed-per-model");
                AssertMultipleConnections(graph, rand, "model-2", i, "b-to-many-a-hashed-per-model");
            }
        }

        [Fact]
        public void SerializationTest()
        {
            NFCompressedGraph compressedGraph = _buildGraph.Compress();

            var outputStream = new MemoryStream();
            compressedGraph.WriteTo(outputStream);

            var inputStream = new MemoryStream(outputStream.ToArray());
            var graph = NFCompressedGraph.ReadFrom(inputStream);

            for (int i = 0; i < NUM_A_NODES; i++)
            {
                foreach (var property in Spec.GetNodeSpec("node-type-a"))
                {
                    var expected = compressedGraph.GetConnectionIterator("node-type-a", i, property.Name);
                    var actual = graph.GetConnectionIterator("node-type-a", i, property.Name);


                    int expectedOrdinal = expected.NextOrdinal();
                    int actualOrdinal = actual.NextOrdinal();

                    while (expectedOrdinal != Consts.NO_MORE_ORDINALS && actualOrdinal != Consts.NO_MORE_ORDINALS)
                    {
                        expectedOrdinal = expected.NextOrdinal();
                        actualOrdinal = actual.NextOrdinal();
                    }

                    Assert.Equal(expectedOrdinal, actualOrdinal);
                }
            }

            for (int i = 0; i < NUM_B_NODES; i++)
            {
                foreach (var property in Spec.GetNodeSpec("node-type-b"))
                {
                    var expected = compressedGraph.GetConnectionIterator("node-type-b", i, property.Name);
                    var actual = graph.GetConnectionIterator("node-type-b", i, property.Name);


                    int expectedOrdinal = expected.NextOrdinal();
                    int actualOrdinal = actual.NextOrdinal();

                    while (expectedOrdinal != Consts.NO_MORE_ORDINALS && actualOrdinal != Consts.NO_MORE_ORDINALS)
                    {
                        expectedOrdinal = expected.NextOrdinal();
                        actualOrdinal = actual.NextOrdinal();
                    }


                    Assert.Equal(expectedOrdinal, actualOrdinal);
                }
            }


        }

        private void AddMultipleRandomConnections(Random rand, NFBuildGraph graph, int fromOrdinal, String model, String propertyName)
        {
            if (rand.NextBool())
            {
                HashSet<int> connections = BuildRandomConnectionSet(rand);
                foreach (int connection in connections)
                {
                    graph.AddConnection(model, "node-type-b", fromOrdinal, propertyName, connection);
                }
            }
        }

        private void AssertMultipleConnections(NFGraph graph, Random rand, String model, int fromOrdinal, String propertyName)
        {
            OrdinalSet set = graph.GetConnectionSet(model, "node-type-b", fromOrdinal, propertyName);

            if (!rand.NextBool())
            {
                Assert.Equal(0, set.Size());
                return;
            }

            HashSet<int> connections = BuildRandomConnectionSet(rand);

            IOrdinalIterator iter = set.Iterator();

            int actualOrdinal = iter.NextOrdinal();
            while (actualOrdinal != Consts.NO_MORE_ORDINALS)
            {
                Assert.True(connections.Contains(actualOrdinal));
                actualOrdinal = iter.NextOrdinal();
            }

            Assert.Equal(connections.Count, set.Size());
        }

        private HashSet<int> BuildRandomConnectionSet(Random rand)
        {
            int numConnections = rand.Next(100);
            var connections = new HashSet<int>();
            for (int j = 0; j < numConnections; j++)
            {
                int connectedTo = rand.Next(NUM_A_NODES);
                while (connections.Contains(connectedTo))
                    connectedTo = rand.Next(NUM_A_NODES);
                connections.Add(connectedTo);
            }
            return connections;
        }



        
    }

    public static class RandomExtensions
    {
        public static bool NextBool(this Random random)
        {
            return random.Next(2) == 0;
        }
    }
}
