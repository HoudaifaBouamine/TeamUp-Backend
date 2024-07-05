<script lang="ts">
	async function selectButton(buttonId: string) {
		const buttons = document.querySelectorAll('.filter-radio-button');
		const texts = document.querySelectorAll('.filter-text');

		// Remove the selected class from all buttons
		buttons.forEach((btn) => btn.classList.remove('bg-primary-500'));
		texts.forEach((btn) => btn.classList.remove('text-white'));

		// Add the selected class to the clicked button
		const selectedButton = document.getElementById(buttonId);
		if (selectedButton == null) return;
		selectedButton.classList.add('bg-primary-500');
		selectedButton.children[1].classList.add('text-white');
	}
</script>

<div class="relative btn-group border border-gray-600 mt-10p">
	<button
		id="months"
		class="filter-radio-button"
		on:click={async () => await selectButton('months')}
	>
		<img src="" alt="" />
		<p class="filter-text">View All</p>
	</button>
	<button id="days" class="filter-radio-button" on:click={async () => await selectButton('days')}>
		<img src="" alt="" />
		<p class="filter-text">Mentors</p>
	</button>
	<button id="years" class="filter-radio-button" on:click={() => selectButton('years')}>
		<img src="" alt="" />
		<p class="filter-text">Confirmed Users</p></button
	>
</div>

<style>
	.btn-selected {
		@apply bg-blue-500 text-white;
	}
</style>
