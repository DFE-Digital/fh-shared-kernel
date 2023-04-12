using FluentAssertions;
using System.Collections.Concurrent;

namespace FamilyHubs.SharedKernel.UnitTests.IdGenerator;

public class IdGen64_NewId
{
    readonly IdGen64 _idGenerator = new IdGen64();
    readonly ConcurrentDictionary<long, long> _ids = new ConcurrentDictionary<long, long>(); // use a concurrent dictionary to store the generated id's, adding a duplicate will return false

    [Fact]
    public void GeneratesNewId()
    {
        var idGenerator = new IdGen64();
        var id = idGenerator.NewId();

        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GeneratesUnqiueNewId()
    {
        var x = 1000000;
        Parallel.For(0, x, i => {
            Parallel.Invoke(CreateNewId);
        });

        _ids.Should().HaveCount(x); // put a breakpoint here and look at the values for the keys to see the different thread ids that created the unique id's
    }
    private void CreateNewId()
    {
        var id = _idGenerator.NewId();

        bool dup = false;
            dup = _ids.TryAdd(id, Thread.CurrentThread.ManagedThreadId);
            if (!dup)
                throw new Exception($"Duplicate: {id}");
    }
}