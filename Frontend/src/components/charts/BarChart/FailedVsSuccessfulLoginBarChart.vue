<script>
  //Importing Bar class from the vue-chartjs wrapper
  import { Bar } from 'vue-chartjs'

  // Services
	import { GetAnalyticsData, FailedvsSucessfulLoginAttempts } from '@/services/analyticsServices';

  const months = ['January','February','March','April','May','June','July','August','September','October','November','December'];

  //Exporting this so it can be used in other components
  export default {
    extends: Bar,
    data () {
      return {
        datacollection: {
          //Data to be represented on x-axis
          labels: [],
          datasets: [
            {
              label: 'Failed vs Successful Login Attempts',
              backgroundColor: [
                            "rgba(255, 99, 132, 0.6)",
                            "rgba(75, 192, 192, 0.6)",
                            "rgba(153, 102, 255, 0.6)",
              ],
              pointBackgroundColor: 'white',
              borderWidth: 1,
              pointBorderColor: '#249EBF',
              //Data to be represented on y-axis
              data: []
            },
            
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
    mounted () {
      this.fetchData();
      //renderChart function renders the chart with the datacollection and options object.
      this.renderChart(this.datacollection, this.options)
    },
    methods: {
      fetchData() {
        GetAnalyticsData()
					.then(response => {
						const rawData = response.data.loginAttempts;
						const data = FailedvsSucessfulLoginAttempts(rawData);
            console.log(data);
						let monthLabels = [];
            let monthData = [];
            data.map(month => {
              monthLabels.push(months[month.date.getMonth()]);
              monthData.push(month.totalRegisteredUsers);
            })
            this.datacollection.labels = monthLabels;
            this.datacollection.datasets[0].data = monthData;
            this.renderChart(this.datacollection, this.options)
					})
      }
    },
  }
</script>