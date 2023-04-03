---
canvasWidth: 1280
theme: default
colorSchema: dark
class: text-center
highlighter: prism
title: Promise API bad, async API good
---

# `Promise` API bad;
# `async` API good

<img src="/steve.jpg" alt="Steve" class="profile" />
<img src="/logo.png" alt="ecoAPM logo" class="logo" />

## Steve Desmond

### April 3, 2023 

---

## What's a `Promise`?

`const task = new Promise(onSuccess, onFailure);`

![Promise workflow via MDN](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise/promises.png)

Queued in event loop


---

## Terms

- Pending (running)
- Fulfilled (succeeded)
- Rejected (failed)
- Settled (finished, succeeded or failed)
- Resolved (settled by another (often inner) promise)

---

## Using `Promise`s

```js
function doSomething() {
	return new Promise((resolve, reject) => {
		const result = slowOperation();
		return result.success
			? Promise.resolve(result.data);
			: Promise.reject("something failed");
		});
}

doSomething()
	.then(data => console.log("here's the data: " + data))
	.catch(error => console.log("here's the error: " + error));
```

---

## Chaining `Promise`s

```js
function doSomething() {
	...
}

function doSomethingElse(data) {
	...
}

function doAThirdThing(data) {
	...
}

doSomething()
	.then(data => doSomethingElse(data))
	.then(otherData => doAThirdThing(otherData));
```

What if we want to do 2 operations on `data`?

---

## Parallelization

```js
doSomething()
	.then(data => {
		const tasks = [
			doSomethingElse(data),
			doAThirdThing(data)
		];
		return Promise.all(tasks);
	});
```

But now how do we handle `catch`ing?

---

## `then`/`catch` chaining confusion

Can we `catch` an intermediate failure and continue? e.g.
```js
doSomething()
	.then(data => doSomethingElse(data))
	.catch(error => handleError(error))
	.then(otherData => doSomethingElse(otherData))
	.catch(otherError => handleError(otherError));
```

No, only one top-level `catch` is supported; the rejection falls through:
```js
doSomething()
	.then(data => doSomethingElse(data))
	.then(otherData => doSomethingElse(otherData))
	.catch(error => handleError(error));
```

---

## `then`/`catch` chaining confusion redux

Or, separate into multiple commands:
```js
const myTask = doSomething()
	.then(data => doSomethingElse(data))
	.catch(error => handleError(error));

myTask
	.then(otherData => doSomethingElse(otherData))
	.catch(otherError => handleError(otherError));
```


---
class: 'sxs'
---

## `async`/`await`

The OGs
- F# (2007)
- C# (2012)
- Haskell (2012)
- Python (2015)
- TS (2015)

The newbs
- JS (2017)
- Rust (2019)
- C++ (2020)
- Swift (2021)
- Perl (?!)

---

## `async`/`await` in JS

- `async function`s return `Promise`s
- `await` waits for a `Promise` to `resolve`
- `await` `throw`s on `reject`
- *mostly* acts like syntactic sugar for `Promise`s
- edge cases with reference equality you can ignore

---
class: 'sxs'
---

## `Promise`s vs `async`/`await`

```js
function doSomething() {
	return new Promise((resolve, reject) => {
		const result = slowOperation();
		return result.success
			? Promise.resolve(result.data);
			: Promise.reject("something failed");
		});
}
}

doSomething()
	.then(data => console.log("here's the data: " + data))
	.catch(error => console.log("here's the error: " + error));
```

```js
async function doSomething() {
	const result = await slowOperation();

	if (result.success)
		return result.data;
	
	throw new Error("something failed");
}

try {
	const data = await doSomething();
	console.log("here's the data: " + data);
}
catch (e) {
	console.log("here's the error: " + e);
}
```

---
class: 'sxs'
---

## Chaining `async`/`await` calls

```js
const result1 = await doSomething();
const result2 = await doSomethingElse(result1);
const result3 = await doAThirdThing(result2);
```

```js
const result1 = await doSomething();
//these run parallelized
const task2 = doSomethingElse(result1);
const task3 = doAThirdThing(result1);

const result2 = await task2;
const result3 = await task3;
```

---

## In Conclusion

### `Promise` API bad; `async` API good

&nbsp;

&nbsp;

[ecoAPM .github.io /presentations](https://ecoAPM.github.io/presentations/)

---
class: 'contact'
---

<img src="/steve.jpg" alt="Steve" />

# Steve Desmond

### Lead Developer @ ecoAPM

<img src="/logo.png" alt="ecoAPM logo" />

<i class="fa fa-globe"></i> [SteveDesmond.ca](https://SteveDesmond.ca) | [ecoAPM.com](https://ecoAPM.com)

<i class="fa fa-github"></i> [SteveDesmond-ca](https://github.com/SteveDesmond-ca) | [ecoAPM](https://github.com/ecoAPM)

<i class="fa fa-twitter"></i> [SteveDesmond_ca](https://twitter.com/SteveDesmond_ca) | [ecoAPM](https://twitter.com/ecoAPM)

<i class="fa fa-envelope-o"></i> [Steve @ ecoAPM .com](mailto:Steve@ecoAPM.com)

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