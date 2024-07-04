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

	hljs.registerLanguage('xml', xml); // for HTML
	hljs.registerLanguage('css', css);
	hljs.registerLanguage('javascript', javascript);
	hljs.registerLanguage('typescript', typescript);
	storeHighlightJs.set(hljs);
</script>

<div class="flex h-screen bg-primary-500 text-on-primary">
	<div class="w-64 p-4 flex flex-col bg-tertiary-500">
		<!-- Logo / Brand -->
		<div class="mb-8">
			<img src="assets/logo.svg" alt="Logo" class="h-12 mx-auto" />
		</div>

		<div class="divide-y">
			<!-- Navigation Links -->
			<nav class="flex-1">
				<button
					type="button"
					class="btn w-full bg-tertiary-500 text-on-secondary hover:bg-primary-400"
				>
					<img src="assets/side-bar/home.svg" alt="wow" />
					<span>Home</span>
				</button>
				<button
					type="button"
					class="btn w-full bg-tertiary-500 text-on-secondary hover:bg-primary-400"
				>
					<img src="assets/side-bar/user.svg" alt="" />
					<span>Users</span>
				</button>
				<button
					type="button"
					class="btn w-full bg-tertiary-500 text-on-secondary hover:bg-primary-400"
				>
					<img src="assets/side-bar/project.svg" alt="wow" />
					<span>Projects</span>
				</button>
				<button
					type="button"
					class="btn w-full bg-tertiary-500 text-on-secondary hover:bg-primary-400"
				>
					<img src="assets/side-bar/mentorship.svg" alt="" />
					<span>Mentorship</span>
				</button>
			</nav>

			<button
				type="button"
				class="btn w-full bg-tertiary-500 text-on-secondary hover:bg-primary-400"
			>
				<img src="assets/side-bar/analytics.svg" alt="" />
				<span>Analytics</span>
			</button>

			<button
				type="button"
				class="btn w-full bg-tertiary-500 text-on-secondary hover:bg-primary-400"
			>
				<img src="assets/side-bar/logout.svg" alt="" />
				<span>Log Out</span>
			</button>
		</div>
	</div>

	<!-- Main Content -->

	<slot />
</div>
