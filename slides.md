---
canvasWidth: 1280
theme: default
colorSchema: dark
class: text-center
highlighter: prism
title: Profiling and Fixing Common Performance Bottlenecks
---

# Profiling and Fixing Common Performance Bottlenecks

<img src="/steve.jpg" alt="Steve" class="profile" />
<img src="/logo.png" alt="ecoAPM logo" class="logo" />

## Steve Desmond

### October 7, 2021

<style>
  img.profile {
    width: 20%;
    float: left;
  }

  img.logo {
    width: 20%;
    float: right;
  }
</style>

---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://stevedesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/stevedesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/stevedesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)

<style>
  img {
    width: 25%;
    float: left;
    margin-right: 4rem;
    clear: both;
  }
</style>

---

## Agenda

1. Types of Bottlenecks
1. Latency Scales
1. Profiler Crash Course
1. Hands-On Code Improvements
1. Q & A

---

## Types of Bottlenecks

- CPU bound
  - memory
- I/O bound
  - disk
  - network

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

---

## Profiling 101

### What?
> Act of measuring the CPU time and/or memory usage of running code

### How?
> .NET CLR (COM) provides profiling API hooks, for third-party tools to aggregate

### Why?
> Easy access to detailed performance data about our application

---

## Profiling Methods

### Sampling
- Least instrusive, least detail; can connect to an already-running application

### Tracing
- Slightly slower, more information

### Line-by-Line
- Most overhead, most detail

### Timeline
- Similar to sampling, but also captures event order

<style>
  * {
    font-size: 75%;
  }

  h3 {
    margin: 1rem 0 0.5rem 0 !important;
  }
</style>

---

# Serial vs Parallel

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

---

# async / await

- great for simplifying code and hiding complexity
- great for easily leaving perf on the table

---

# async parallelization

```csharp
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var result = await GetResult(x);
  var results.Add(result);
}

return results;
```

vs

```csharp
var tasks = Enumerable.Range(1, 5).Select(GetResult).ToArray();
var results = await Task.WhenAll(tasks);
return results;
```

---

# Variable scope in loops

```csharp
var results = new List<int>();

for (var x = 1; x <= 5; x++)
{
  var y = GetY();
  var result = GetResult(x, y);
}

return results;
```

vs

```csharp
var results = new List<int>();
var y = GetY();

for (var x = 1; x <= 5; x++)
{
  var result = GetResult(x, y);
}

return results;
```

<style>
  pre.slidev-code {
    width: 40%;
    float: left;
  }

  p {
    width: 20%;
    float: left;
    text-align: center;
  }
</style>

---

# 

---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://stevedesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/stevedesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/stevedesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)

<style>
  img {
    width: 25%;
    float: left;
    margin-right: 4rem;
    clear: both;
  }
</style>
