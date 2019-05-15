<script>
  //Importing Bar class from the vue-chartjs wrapper
  import { Bar } from 'vue-chartjs'

  // Services
	import { GetAnalyticsData, GetTopPagesByPageTime } from '@/services/analyticsServices';


  //Exporting this so it can be used in other components
  export default {
    name: 'Top5AvgTimeSpentPerPageBarChart',
    extends: Bar,
    data () {
      return {
        datacollection: {
          //Data to be represented on x-axis
          labels: [],
          datasets: [
            {
              label: 'Top 5 Webpages by User Time Spent (Seconds)',
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
    methods:
    {
      fetchData() {
        GetAnalyticsData()
					.then(response => {
						const rawData = response.data.topFeaturesByPageTime;
						const data = GetTopPagesByPageTime(rawData);
						let pageLabels = [];
            let pageData = [];
            data.map(page => {
              pageLabels.push(page.pageName);
              pageData.push(page.duration);
            })
            this.datacollection.labels = pageLabels;
            this.datacollection.datasets[0].data = pageData;
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