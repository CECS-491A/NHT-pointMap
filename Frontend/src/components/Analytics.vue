<template>
  <v-container >
    <v-layout id="analytics">
      <div id="analytics">
        <h1 class="display-1">Usage Analytics</h1>
        <v-divider class="my-3"></v-divider>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <AvgSessionDurationBarChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Average Session Duration</div>
            </v-card-text>
          </v-card>
        </v-flex>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <AvgSessionDurationLineChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Average Session Duration</div>
            </v-card-text>
          </v-card>
        </v-flex>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <AvgSuccessfulLoginBarChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Successful Logins</div>
            </v-card-text>
          </v-card>
        </v-flex>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <FailedVsSuccessfulLoginBarChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Login Attempts</div>
            </v-card-text>
          </v-card>
        </v-flex>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <Top5AvgTimeSpentPerPageBarChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Top 5 Webpages by User Time Spent</div>
            </v-card-text>
          </v-card>
        </v-flex>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <Top5MostUsedFeatureBarChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Top 5 Features by Page Visits </div>
            </v-card-text>
          </v-card>
        </v-flex>
        
        <v-flex class="analyticCard">
          <v-card
            class="mx-auto text-xs-center"
            color="white"
            max-width="800"
          >
            <v-card-text>
              <v-sheet color="grey lighten-4" elevation="6">
                <NumOfLoggedInUsersLineChart/>
              </v-sheet>
            </v-card-text>
            <v-card-text>
              <v-divider class="my-1"></v-divider>
              <div class="display-1 font-weight-light">Total Logins</div>
            </v-card-text>
          </v-card>
        </v-flex>
        
      </div>
    </v-layout>
  </v-container>
</template>

<script>
  import axios from 'axios';
  
  import AvgSuccessfulLoginBarChart from '@/components/charts/BarChart/AvgSuccessfulLoginBarChart'
  import AvgSessionDurationBarChart from '@/components/charts/BarChart/AvgSessionDurationBarChart'
  import FailedVsSuccessfulLoginBarChart from '@/components/charts/BarChart/FailedVsSuccessfulLoginBarChart'
  import Top5AvgTimeSpentPerPageBarChart from '@/components/charts/BarChart/Top5AvgTimeSpentPerPageBarChart'
  import Top5MostUsedFeatureBarChart from '@/components/charts/BarChart/Top5MostUsedFeatureBarChart'
  import AvgSessionDurationLineChart from '@/components/charts/LineChart/AvgSessionDurationLineChart'
  import NumOfLoggedInUsersLineChart from '@/components/charts/LineChart/NumOfLoggedInUsersLineChart'  
  
  export default {
    name: 'Analytics',
    components: {
      // Bar Chart 
      AvgSuccessfulLoginBarChart,
      AvgSessionDurationBarChart,
      FailedVsSuccessfulLoginBarChart,
      Top5AvgTimeSpentPerPageBarChart,
      Top5MostUsedFeatureBarChart,
      
      // Line Chart
      AvgSessionDurationLineChart,
      NumOfLoggedInUsersLineChart
    },
    data() {
      return {
        loaded: false,
        data: [],
        label: []
      }
    },
  }

</script>

<style>
#analytics{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 900px;
  text-align: center;
  margin: 1px auto;	  
  align: center;
}

.analyticCard {
  margin-top: 50px;
}
</style>