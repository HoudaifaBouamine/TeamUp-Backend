<script lang="ts">
	import '../app.postcss';

	// Highlight JS
	import hljs from 'highlight.js/lib/core';
	import 'highlight.js/styles/github-dark.css';
	import { storeHighlightJs } from '@skeletonlabs/skeleton';
	import xml from 'highlight.js/lib/languages/xml'; // for HTML
	import css from 'highlight.js/lib/languages/css';
	import javascript from 'highlight.js/lib/languages/javascript';
	import typescript from 'highlight.js/lib/languages/typescript';
	import { computePosition, autoUpdate, offset, shift, flip, arrow } from '@floating-ui/dom';
	import { storePopup } from '@skeletonlabs/skeleton';
	import { page } from '$app/stores';
	storePopup.set({ computePosition, autoUpdate, offset, shift, flip, arrow });

	hljs.registerLanguage('xml', xml); // for HTML
	hljs.registerLanguage('css', css);
	hljs.registerLanguage('javascript', javascript);
	hljs.registerLanguage('typescript', typescript);
	storeHighlightJs.set(hljs);
	import {
		AppBar,
		Autocomplete,
		popup,
		type AutocompleteOption,
		type PopupSettings
	} from '@skeletonlabs/skeleton';

	let inputDemo = '';

	const flavorOptions: AutocompleteOption<string>[] = [
		{ label: 'Vanilla', value: 'vanilla', keywords: 'plain, basic', meta: { healthy: false } },
		{ label: 'Chocolate', value: 'chocolate', keywords: 'dark, white', meta: { healthy: false } },
		{ label: 'Strawberry', value: 'strawberry', keywords: 'fruit', meta: { healthy: true } },
		{
			label: 'Neapolitan',
			value: 'neapolitan',
			keywords: 'mix, strawberry, chocolate, vanilla',
			meta: { healthy: false }
		},
		{ label: 'Pineapple', value: 'pineapple', keywords: 'fruit', meta: { healthy: true } },
		{ label: 'Peach', value: 'peach', keywords: 'fruit', meta: { healthy: true } }
	];

	function onFlavorSelection(event: CustomEvent<AutocompleteOption<string>>): void {
		inputDemo = event.detail.label;
	}

	let popupSettings: PopupSettings = {
		event: 'focus-click',
		target: 'popupAutocomplete',
		placement: 'bottom'
	};

	let inputPopupDemo: string = '';

	const onPopupDemoSelect = (e: CustomEvent) => {
		console.log(e.detail);
		inputPopupDemo = e.detail.label;
	};
</script>

<div class="flex h-screen bg-primary-500 text-on-primary">
	<div class="w-64 p-4 flex flex-col bg-secondary-100">
		<!-- Logo / Brand -->
		<div class="mb-8">
			<img src="assets/logo.svg" alt="Logo" class="h-12 mx-auto" />
		</div>

		<div class="divide-y divide-secondary-300">
			<!-- Navigation Links -->
			<nav class="flex-1">
				<button
					type="button"
					class="btn w-full bg-secondary-100 text-on-tertiary-token hover:bg-primary-400"
				>
					<img src="/assets/side-bar/home.svg" alt="wow" />
					<span>Home</span>
				</button>
				<button
					type="button"
					class="btn w-full bg-secondary-100 text-on-tertiary-token hover:bg-primary-400"
				>
					<img src="/assets/side-bar/user.svg" alt="" />
					<span>Users</span>
				</button>
				<button
					type="button"
					class="btn w-full bg-secondary-100 text-on-tertiary-token hover:bg-primary-400"
				>
					<img src="/assets/side-bar/project.svg" alt="wow" />
					<span>Projects</span>
				</button>
				<button
					type="button"
					class="btn w-full bg-secondary-100 text-on-tertiary-token hover:bg-primary-400"
				>
					<img src="/assets/side-bar/mentorship.svg" alt="" />
					<span>Mentorship</span>
				</button>
			</nav>

			<div>
				<button
					type="button"
					class="btn w-full bg-secondary-100 text-on-tertiary-token hover:bg-primary-400"
				>
					<img src="/assets/side-bar/analytics.svg" alt="" />
					<span>Analytics</span>
				</button>

				<button
					type="button"
					class="btn w-full bg-secondary-100 text-on-tertiary-token hover:bg-primary-400"
				>
					<img src="/assets/side-bar/logout.svg" alt="" />
					<span>Log Out</span>
				</button>
			</div>
		</div>
	</div>

	<!-- Main Content -->

	<div class="flex flex-col bg-white">
		<div class="mx-[24px] my-[22px]">
			<input
				class="input autocomplete"
				type="search"
				name="autocomplete-search"
				bind:value={inputPopupDemo}
				placeholder="Search..."
				use:popup={popupSettings}
			/>
			<div data-popup="popupAutocomplete">
				<Autocomplete
					bind:input={inputPopupDemo}
					options={flavorOptions}
					on:selection={onPopupDemoSelect}
				/>
			</div>
		</div>

		<div class="mx-[23px] my-[20px">
			<h1 class="text-[32px] font-[700]">
				{$page.url.pathname.split('/').at(-1)}
			</h1>
			<br />
			<slot />
		</div>
	</div>
</div>
