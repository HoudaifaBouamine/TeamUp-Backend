<script lang="ts">
	export let rows;
</script>

<!-- Responsive Container (recommended) -->
<div class="table-container">
	<!-- Native Table Element -->
	<table class="table table-hover table-compact">
		<thead>
			<tr>
				<th>Pic</th>
				<th>Display Name</th>
				<th>Id</th>
				<th>Email</th>
				<th>Mentorship</th>
				<th>Email Status</th>
			</tr>
		</thead>
		<tbody>
			{#each rows as row, i}
				<tr>
					<td class="flex items-center justify-center"
						><img class=" w-[40px] rounded-full" src={row.profilePicture} alt="wow" /></td
					>
					<td><p class="text-start">{row.displayName}</p></td>
					<td><p>{row.id}</p></td>
					<td><p>{row.email}</p></td>
					<td><p>{row.mentorship}</p></td>
					<td><p>{row.emailStatus}</p></td>
				</tr>
			{/each}
		</tbody>
		<tfoot>
			<tr>
				<th colspan="3">Calculated Total Weight</th>
				<td>{1}</td>
			</tr>
		</tfoot>
	</table>
</div>

<style>
	p {
		@apply text-[14px] m-2 text-justify;
	}

	td {
		@apply bg-white;
	}

	tr {
		@apply bg-[#EEEEEE];
	}
</style>
