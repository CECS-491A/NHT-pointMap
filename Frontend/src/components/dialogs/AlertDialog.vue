<template> 
	<v-dialog v-model="thisDialog" max-width="500px">
		<v-card>
			<v-card-title>
					<span>{{text}}</span>
					<v-spacer></v-spacer>
			</v-card-title>
			<v-card-actions>
			<v-btn color="primary" flat @click="thisDialog=false">Close</v-btn>
			</v-card-actions>
		</v-card>
	</v-dialog>
</template>

<script>
export default {
    name: 'AlertDialog',
    props: ['dialog', 'text'],
		data: () => ({
			thisDialog: this.dialog
		}),
		created(){
			this.thisDialog = this.dialog
		}
}
</script>