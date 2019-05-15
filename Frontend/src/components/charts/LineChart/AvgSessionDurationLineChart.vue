<script>
  //Importing Line class from the vue-chartjs wrapper
  import { Line } from 'vue-chartjs';
  import axios from 'axios';

  // Import Services
  import { GetAnalyticsData, GetAverageSessionDuration6Months } from '@/services/analyticsServices';

  const months = ['January','February','March','April','May','June','July','August','September','October','November','December'];

  //Exporting this so it can be used in other components
  export default {
    extends: Line,
    data () {
      return {
        loading: false,
        datacollection: {
          //Data to be represented on x-axis
          labels: [],
          datasets: [
            {
              label: 'Average Session Duration',
              backgroundColor: "grey",
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
    created(){
      this.fetchData();
    },
    mounted () {
      //renderChart function renders the chart with the datacollection and options object.
      this.renderChart(this.datacollection, this.options)
    },
    methods:
    {
      fetchData() {
        this.loading = true;
        GetAnalyticsData()
          .then(response => {
            const data = response.data.averageSessionDuration6Months;
            const averageDurationData = GetAverageSessionDuration6Months(data);
            let monthLabels = [];
            let monthData = [];
            averageDurationData.map(month => {
              monthLabels.push(months[month.date.getMonth()]);
              monthData.push(month.duration);
            })
            this.datacollection.labels = monthLabels;
            this.datacollection.datasets[0].data = monthData;
            this.renderChart(this.datacollection, this.options)
          })
      }
    },
  }
</script>