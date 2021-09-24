---
canvasWidth: 1280
aspectRatio: 1.6
theme: default
colorSchema: dark
background: fallout.jpg
highlighter: prism
title: Profiling and Fixing Common Performance Bottlenecks
---

#

<!--
Setup
- charge Bluetooth headset
- turn off dehumidifier
- right monitor to 1280x800
- reset to initial app commit (keep git log open on main)
- delete DB table (keep ADS open on main)
- open Rider on right, run app w/o debugging
- open dotTrace on right, leave on main menu
- open Ubuntu on right, run `clear`
- Firefox:
  - slides on right, presenter view on main, anchored right-2/3, StreamYard anchored left-1/3
  - new window with app on right
-->

---
layout: cover
class: text-center
---

# Profiling and Fixing Common Performance Bottlenecks

<img src="/steve.jpg" alt="Steve" class="profile" />
<img src="/logo.png" alt="ecoAPM logo" class="logo" />

## Steve Desmond

### October 7, 2021

<!--
- welcome
- excited to get to talk about one of favorite things
-->

---
class: contact
---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://SteveDesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/SteveDesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/SteveDesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)

<!--
- a little about me
- building software professionally for almost 2 decades
- now lead an open source software company focusing on performance
- here's how to find me online
-->

---

## Agenda

1. Latency & Bottlenecks
1. Profiling & Profilers
1. Hands-On Part 1
1. Load Testing
1. Hands-On Part 2 
1. Q & A

<!--
- "So what are we going to talk about today?"
- go through list
- can also ask questions throughout and I'll answer as we go
- also note while examples are all C#, a lot of this holds true anywhere
-->

---

## Why should we care about *performance*?

- user experience
- accessibility
- hosting costs
- planned obsolescence
- energy usage

<!--
- "But first" ask question, 5 reasons (not only ones)
- UX: studies introducting artificial latency reduced sales, removing artificial latency increased sales
- accessibility: cognitive differences, but also economic
- bottom line: (why company cares) less scaling, hardware requirements
- planned obsolescence: big problem, planet has finite resources
- energy: software eating world, using vast amounts of electricity
- <10 countries get majority of electricity from renewables
- don't get started on bitcoin
-->

---

## Latency vs Throughput

Latency:
> how fast can you return a response

Throughput:
> how many responses can you return

<!--
- 2 main metrics w/r/t web perf
- focusing almost exclusively on latency today
- when you do that, you get throughput for free
- focus on throughput = "scale out" / throw more hardware at it
- that adds complexity: load balancers, etc.
-->

---

## Computer Latency

| CPU instruction	| 0.5 ns |
|-|-:|
| L1 cache | 1 ns |
| L2 cache | 4 ns |
| L3 cache | 15 ns |
| RAM | 30 ns |
| SSD | 30 μs |
| LAN (1 Gbps) | 1 ms |
| Net (Azure BB) | 6 ms |

Sources
: Intel ([1](https://software.intel.com/content/www/us/en/develop/articles/memory-performance-in-a-nutshell.html))
; AnandTech ([1](https://www.anandtech.com/show/8423/intel-xeon-e5-version-3-up-to-18-haswell-ep-cores-/11), [2](https://www.anandtech.com/bench/SSD21/2994))
; Microsoft ([1](https://docs.microsoft.com/en-us/azure/networking/azure-network-latency))

<!--
- need to get into some numbers to understand the sizes of certain things
- go through list, point out jumps in magnitude
- note global internet ping times (up to 500ms)
-->

---

## On a Human Time Scale

| CPU instruction	| 1 sec |
|-|-:|
| L1 cache | 2 sec |
| L2 cache | 8 sec |
| L3 cache | 30 sec |
| RAM | 1 min | 
| SSD | 9 hours |
| LAN (1 Gbps) | 12 days |
| Net (Azure BB) | 10 weeks |

<!--
- "Since humans have a hard time with very small and very large scales"
- note everything multiplied by 2 billion
- go through list
- easier to see jumps between latter items
-->

---

## Types of Bottlenecks

- CPU bound
  - algorithms
  - memory efficiency
- I/O bound
  - disk
  - network

<!--
- "And so, based on this divide, we have 2 types of bottlenecks:"
- CPU about algorithms and memory, especially in .NET and other managed runtimes
- I/O about efficient offloading of async tasks, so CPU can keep working while it waits for data
- example of container ship across ocean, what to do in meantime 
- different solutions for each, which we'll go through
-->

---

## Profiling 101

### What?
> The act of measuring the CPU time and/or memory usage of running code

### Why?
> Easy access to detailed performance data about our application

### How?
> .NET CLR (COM) provides profiling API hooks, for third-party tools to aggregate

<!--
- "But how do we know *what's* slow?"
- lots of ways, profilers are one tool in our toolbox
- go through what/why/how
- after how: "dotTrace is my .NET profiler of choice"
-->

---
class: 'small'
---

## Profiling Methods

### Sampling
> Least instrusive, least detail; can connect to an already-running application

### Tracing
> Slightly slower, more information

### Line-by-Line
> Most overhead, most detail

### Timeline
> Similar to sampling, but also captures event order and therefore can best show async/await details

<!--
- dotTrace has 4 methods (go through list)
- usually start w/timeline to get overall visual, then dive into LxL
- that's what we're going to do today
-->

---

## Hands-On Part 1

1. app overview
1. profile a slow page
1. improve its performance
1. rinse & repeat

<!--
- go over list

Firefox: app
- show main page & phone page
- give MVP (24s, 172 requests, 4MB) & Wegmans (20s, 459 requests, 20MB) examples for realism

dotTrace
- explain main menu
- select timeline, check TPL box, start profiling, refresh demo app phone page
- load profile, explain window, introduce parallelization

Firefox: slides
-->

---

## Serial vs Parallel

```yml
- 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32
```
vs
```yml
- 01 02 03 04 05 06 07 08
- 09 10 11 12 13 14 15 16
- 17 18 19 20 21 22 23 24
- 25 26 27 28 29 30 31 32
```
(or)
```yml
- 01 05 09 13 17 21 25 29
- 02 06 10 14 18 22 26 30
- 03 07 11 15 19 23 27 31
- 04 08 12 16 20 24 28 32
```

<!--
- here's example w/32 tasks
- we could do them all in order, or break them up into 4 groups
- note order doesn't matter here
- could split them more
-->

---
class: 'sxs'
---

## async / await

- C#
- F#
- Haskell
- Python
- TS
- JS
- Rust
- C++
- Swift
- Perl

Great for simplifying code and hiding complexity

Great for easily leaving perf on the table

"async-ifying" ™

<!--
- very familiar w/async/await in .NET, but the keywords exist in many languages now
- while great for #1, also leads to #2
- especially true when "async-ifying"
- ...what is "async-ifying"?
-->

---

![async all the things!](/async.jpg)

<style>
  img {
    width: 100%;
    aspect-ratio: 16/9;
    object-fit: contain;
  }
</style>

<!--
- when you're like, "async all the things"
- sprinkle async/await keywords everywhere, but don't change anything else
- great, unblocked threads while I/O happens
-->

---

## `Task` parallelization

```csharp {all|3-7|5}
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var result = await GetResult(x);
  var results.Add(result);
}

return results;
```

vs

```csharp {all|1|1-2}
var tasks = Enumerable.Range(1, 5).Select(GetResult);
var results = await Task.WhenAll(tasks);
return results;
```

.NET 6: `Parallel.ForEachAsync()`

<!--
- simplified example of what we found
- explain comparison

Rider: HomeController
- Extract GetPriceViewModel(), parallelize usage
- re-run w/o debugging

dotTrace
- timeline profile page reload
- look at improvement, but still sequential

Firefox: slides
-->

---

## Asynchronous "pre-fetch"

```csharp {all|3-4}
return new Result
{
  X = await GetX(),
  Y = await GetY()
};
```

vs

```csharp {all|1-2|1-2,6-7}
var x = GetX();
var y = GetY();

return new Result
{
  X = await x,
  Y = await y
};
```

<!--
Rider: HomeController
- extract logo and price task variables
- re-run, show improvements
- re-profile, compare timelines, note additional threads

Firefox: slides
-->

---
class: sxs
---

## Eliminate Repeat Actions

```csharp {all|1-2,6-7}
var x = GetX();
var y = GetY();

return new Result
{
  X = await x,
  Y = await y
};
```

←

```csharp {all|3,9}
async Task<int> GetX()
{
  var value = await GetValue();
  return value.X;
}

async Task<int> GetY()
{
  var value = await GetValue();
  return value.Y;
}
```

### vs

```csharp {all|1,5-6}
var value = await GetValue();

return new Result
{
  X = value.X,
  Y = value.Y
};
```

<!--
- "But what happens when tasks use the same underlying data?"
- here's where we're currently at
- `GetX()` and `GetY()` are calling the same thing under the hood
- getting this information twice, let's fix that

Rider: HomeController
- move GetPriceViewModel to PriceDisplayService
- explain static + sync improvements as we go

dotTrace: same-ish time but half the CPU usage

Firefox: slides
-->

---
layout: cover
class: text-center
---

# Interlude

<!--
- all `async`/`await` so far
- any questions about `async`/`await`?
- clean up dotTrace instances
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Use The Best Tools For The Job

<!--
- ready for a LxL profile
- Rider: stop run, start LxL profile, profile info page

dotTrace
- explain new layout, LxL tab, total time vs own
- note heavy AngleSharp

Rider:
- add HttpClient, Factory in PriceDisplayService
- also note favicon logic, BB API endpoint

TortoiseGit: reset to HttpClient

Azure Data Studio: delete DB table

Rider: run app w/o debugging

Firefox: app
- reload page, note improvement

Firefox: slides
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Profile Your Unit Tests

<!--
- not just profile, but profile regularly

Rider
- profile unit tests (LxL), open in dotTrace
- sort by calls to show DB inefficiency

Rider: PhoneInfoService
- fix queries

Firefox: slides
-->

---

## Database Query Optimization

```csharp
var results = await db.QueryAsync<Result>("SELECT * FROM Results");
return results.FirstOrDefault(r => r.ID == 123);
```

vs

```csharp
return await db.QuerySingleAsync<Result>("SELECT * FROM Results WHERE ID = @ID", new { ID = 123 });
```

<hr/>

```csharp
var results = await db.QueryAsync<Result>("SELECT * FROM Results");
return results.Select(r => r.Name).Distinct();
```

vs

```csharp
return await db.QueryAsync<string>("SELECT DISTINCT Name FROM Results");
```

<!--
- may seem obvious to some, but actually quite common
- especially in .NET w/LINQ & EF, lines start to blur where things run
- "SQL is built for this, let it do its thing"
-->

---

## Caching

### What?
> Saving a local copy of data

### Why?
> So you don't need to retrieve it (or calculate it) again

### How?
> `static` field, `IDictionary`, `IMemoryCache`

<!--
- "Which brings us to caching, which as we all know, is one of the 3 hardest problems in computer science, along with naming things, off-by-one errors, and not using this joke whenever you talk about caching"
- read description, give latency example in human time
- why save till later? make sure still fast on cache miss: "defence in depth"
-->

---

## Caching Example

```csharp {all|4}
public class AlwaysSlow
{
  public async Task<ulong> GetDouble()
    => await SlowMethod() * 2;
}
```

vs

```csharp {all|3|6,9|8|6-10}
public class OnlySlowTheFirstTime
{
  private IMemoryCache _cache;

  public async Task<ulong> GetDouble()
    => await _cache.GetOrCreateAsync("ValueDoubled",
        async entry => {
          entry.AbsoluteExpiration = TimeSpan.FromMinutes(1);
          return await SlowMethod() * 2;
        });
}
```

<!--
- go through example

Rider: PriceDisplayService
- extract HTTP GET to GetResponse()
- add AddMemoryCache() to Startup
- add IMemoryCache to PriceDisplayService
- wrap GetResponse() in _cache.GetOrCreateAsync()
- run w/o debugging, show improvement
-->

---
layout: cover
class: text-center
---

# Interlude

<!--
- ask for more questions
-->

---

## Load Testing

### What?
> The act of putting demand on a software system and measuring its response 

### Why?
> To observe how the system reacts to certain volumes of requests

### How?
> `for` loops, `wrk`, [`aspnet/Benchmarks`](https://github.com/aspnet/Benchmarks), [`LoadTestToolbox`](https://github.com/ecoAPM/LoadTestToolbox)

<!--
- now that we're caching HTTP responses, we can take the next step
- go through what/why/how
- describe LTT
-->

---

## Hands-On Part 2

1. run a load test
1. analyze the output
1. profile a load test
1. improve app performance
1. rinse & repeat

<!--
- go over next steps

Rider: start LxL profiling

Ubuntu: drill Moto G page 10rps x 5s, save to /mnt/x/ecoAPM/presentations

dotTrace: see repeated DB calls at top, cache them
- re-run profiler, see GetImageData
- async it, re-run profiler
- "but wait...what did we just do?"
-->

---

![async all the things!](/async.jpg)

<style>
  img {
    width: 100%;
    aspect-ratio: 16/9;
    object-fit: contain;
  }
</style>

<!--
- we "async-ified" it...is there a better way?

Rider: HomeController
- migrate to Image() method
- HTTP+HTML: another "best tool for the job"
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Optimize DI Container Lifetimes

<!--
- this one's a little .NET-specific
- explain scoped vs transient vs singleton
- want to only instantiate as much as necessary

dotTrace
- LxL profile a load test, note multiple AngleSharp instantiations

Rider: Startup
- convert browser/display to singleton
- convert PhoneInfoService to transient
-->

---
class: 'sxs'
---

## Variable scope in loops

```csharp {all|3-7|5}
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var y = GetY();
  var result = GetResult(x, y);
}

return results;
```

vs

```csharp {all|2-7|2}
var results = new List<int>();
var y = GetY();

for (var x = 1; x <= 5; x++)
{
  var result = GetResult(x, y);
}

return results;
```

<!--
- something to watch out for, especially w/larger data sets

Rider: HomeController
- move avg method outside of loop
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Layer Your Caching

<!--
- similar to mention of "defence in depth" before
- if result is not changing between requests, we can cache it
- can use different intervals

Rider: HomeController
- cache PriceViewModel
-->

---
layout: cover
class: 'text-center'
---

# Pro-Tip

## Stay Up-To-Date

<!--
- lastly, let's update to .NET 5
- cause guess what, we've been on 3.1 this whole time
- not going to YOLO v6 yet
- similar to caching, this should be bonus

Rider: csproj files
- update netcoreapp3.1 to net5.0
- run load test against release build

Firefox: slides
-->

---

## Review

From >25,000 ms to <1 ms:
- parallelized our work
- multitasked while async code was running
- eliminated duplicated effort
- optimized DB queries
- offloaded heavy task to new HTTP request
- cached multiple layers of data / calculations

<!--
- go through list
-->

---

## Resources

### Slides

> [https://github.com/ ecoAPM / presentations](https://github.com/ecoAPM/presentations)

### LoadTestToolbox

> [https://github.com/ ecoAPM / LoadTestToolbox](https://github.com/ecoAPM/LoadTestToolbox)

<!--
- here are some links
- confirm w/Khalid that video will be up in a few days
-->

---
layout: cover
class: 'text-center'
---

# Thanks!

<!--
- thanks for coming, thanks to JetBrains for hosting
- hope I've given you some tools and techniques to tackle any performance problems you're experiencing
- if you've tried everything here and still need help,
  if you need a hand getting started,
  or if you don't even know *where to start* with your app,
  I'd love to chat
-->

---
class: 'contact'
---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://stevedesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/stevedesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/stevedesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)

<!--
- here's my info again to get in touch
- not just one of favorite things to talk about, also work on
- thanks again, open it up to questions
-->
