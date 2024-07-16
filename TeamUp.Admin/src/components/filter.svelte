<script lang="ts">
	export let selected: string = 'unselected';
	export let choices: Choice[];

	const select = (selectedName: string) => {
		selected = selectedName;
	};

	type Choice = {
		name: string;
		filterFunc: string;
	};
</script>

<div class="flex flex-row items-center">
	{#each choices as choice, i}
		<input
			type="radio"
			on:select={() => select(choice.name)}
			id={choice.name}
			name="filter"
			class="hidden"
			checked={i === 0}
		/>
		<label
			for={choice.name}
			class="{i == 0 ? 'rounded-l-full' : ''} {i == choices.length - 1
				? 'rounded-r-full'
				: ''} mb-2 p-2 border border-gray-300 cursor-pointer bg-white transition-colors"
		>
			{choice.name}</label
		>
	{/each}
</div>

<style>
	input[type='radio']:checked + label {
		@apply bg-[#774AFF] text-white;
	}

	label {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 130px;
		height: 38px;
		font-size: 13px;
	}
</style>
