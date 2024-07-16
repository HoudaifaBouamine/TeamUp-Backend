<script lang="ts">
	import Filter from '../../components/filter.svelte';
	import Table from '../../components/table.svelte';

	const getUsers = () => {
		// fetch data from "http://localhost:5168/api/v1/users" and get from the body UsersList object type :
		fetch('http://localhost:5168/api/v1/users?PageSize=10&PageNumber=1')
			.then((response) => response.json())
			.then((data) => {
				console.log(data);
				usersList = data;
			});
	};

	$: usersList = undefined as UsersList | undefined;

	type UsersList = {
		totalCount: number;
		pageNumber: number;
		pageSize: number;
		isPrevPageExist: boolean;
		isNextPageExist: boolean;
		users: User[];
	};

	type User = {
		id: string;
		email: string;
		displayName: string;
		handler: string;
		rate: number;
		profilePicture: string;
	};

	const mapUsers = () => {
		if (usersList == undefined || usersList.users == undefined) return [];

		return usersList.users.map((user) => {
			return {
				id: user.id,
				email: user.email,
				displayName: user.displayName,
				handler: user.handler,
				rate: user.rate,
				profilePicture: user.profilePicture
			};
		});
	};
</script>

<div on:load={getUsers}></div>

<Filter
	choices={[
		{ name: 'View All', filterFunc: '' },
		{ name: 'Mentors', filterFunc: '' },
		{ name: 'Confirmed Users', filterFunc: '' }
	]}
/>

<!-- this table should contain those cols : Display Name, Id, Email, Mentorship (value : Verified, Pending, Not Applied), Email Status (values : confirmed, pernding) and an email picture -->

<Table rows={mapUsers()} />
