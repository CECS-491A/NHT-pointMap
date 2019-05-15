<script>
  //Importing Line class from the vue-chartjs wrapper
  import { Line } from 'vue-chartjs'
  import axios from 'axios'

	// Services
	import { GetAnalyticsData, GetSuccessfulLoginsxRegisteredUsers } from '@/services/analyticsServices';

	const months = ['January','February','March','April','May','June','July','August','September','October','November','December'];

  //Exporting this so it can be used in other components
  export default {
    extends: Line,
    props: {
      chartData: {
        type: Array,
      },
      chartLabels: {
        type: Array,
      }
    },
    data () {
      return {
        datacollection: {
          //Data to be represented on x-axis
          labels: [],
          datasets: [
            {
              label: 'Sucessful Logins and Registrations',
              backgroundColor: "rgba(54, 162, 235, 0.6)",
              pointBackgroundColor: 'white',
              borderWidth: 1,
              pointBorderColor: '#249EBF',
              //Data to be represented on y-axis
              data: []
            },
						 {
              label: 'Total Registered Users',
              backgroundColor: "grey",
              pointBackgroundColor: 'white',
              borderWidth: 1,
              pointBorderColor: '#249EBF',
              //Data to be represented on y-axis
              data: []
            }
          ]
        },
        //Chart.js options that controls the appearance of the chart
        options: {
          scales: {
            yAxes: [{
              ticks: {
                beginAtZero: true
              },
              gridLines: {
                display: true
              }
            }],
            xAxes: [ {
              gridLines: {
                display: false
              }
            }]
          },
          legend: {
            display: true
          },
          responsive: true,
          maintainAspectRatio: false
        }
      }
    },
    methods:
    {
      fetchData() {
        GetAnalyticsData()
					.then(response => {
						const rawData = response.data.successfulLoginsxRegisteredUsers;
						const data = GetSuccessfulLoginsxRegisteredUsers(rawData);
						let monthLabels = [];
            let monthData = [ [], [] ];
            data.map(month => {
              monthLabels.push(months[month.date.getMonth()]);
              monthData[0].push(month.loginAttempts);
							monthData[1].push(month.totalRegisteredUsers)
            })
            this.datacollection.labels = monthLabels;
            this.datacollection.datasets[0].data = monthData[0];
						this.datacollection.datasets[1].data = monthData[1];
            this.renderChart(this.datacollection, this.options)
					})
      }
    },
		created() {
			this.fetchData();
		},
     mounted () {
      //renderChart function renders the chart with the datacollection and options object.
      this.renderChart(this.datacollection, this.options)
    }
  }
</script>