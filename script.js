async function getWho(c) {
	const attURL = c.querySelector('a').href.replace('/?eventOrigin=group_events_list', '/attendees');
	const response = await fetch(attURL);
	const text = await response.text();
	const dom = new DOMParser().parseFromString(text, 'text/html');
	const people = Array.from(dom.querySelectorAll('button[data-event-label="attendee-card"]'));
	const names = people.map(p => p.querySelector('h3').innerText);
	return names;
}

async function getElement(c) {
	return {
		name: c.querySelector('h3').innerText,
		date: c.querySelector('time').getAttribute('datetime'),
		location: c.querySelector('.text-ds2-text-fill-tertiary-enabled span.truncate').innerText,
		attendees: c.querySelector('span.ds2-m14')?.innerText,
		who: await getWho(c)
	};
}

const elements = document.querySelectorAll('[data-element-name="group-events-card"]');
const tasks = Array.from(elements).map(async (c) => await getElement(c));
const result = await Promise.all(tasks);
return result;