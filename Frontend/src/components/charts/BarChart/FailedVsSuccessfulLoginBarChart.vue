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
              label: 'Successful Logins',
              backgroundColor: "#7C8CF8",
              borderWidth: 1,
              //Data to be represented on y-axis
              data: []
            },
            {
              label: 'Unsuccessful Logins',
              backgroundColor: '#f87979',
              borderWidth: 1,
              //Data to be represented on y-axis
              data: []
            },
            {
              label: 'Total Login Attempts',
              backgroundColor: "grey",

              //Data to be represented on y-axis
              data: []
            }
          ],

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
						let monthLabels = [];
            let monthData = [ [], [], [] ];
            data.map(month => {
              monthLabels.push(months[month.date.getMonth()]);
              const datum = {
                total: month.totalAttempts,
                successes: month.loginAttempts,
                fails: month.failedAttempts
              }
              monthData[0].push(datum.successes);
              monthData[1].push(datum.fails);
              monthData[2].push(datum.total);
            })
            this.datacollection.labels = monthLabels;

            // Login Data
            this.datacollection.datasets[0].data = monthData[0];

            // Fails Data
            this.datacollection.datasets[1].data = monthData[1];

            // Total Data
            this.datacollection.datasets[2].data = monthData[2];
            this.renderChart(this.datacollection, this.options)
					})
      }
    },
  }
</script>