<template>
  <div>
    <h2>Analytics</h2>
    <AvgSessionDurationBarChart/>
    <AvgSuccessfulLoginBarChart/>
    <FailedVsSuccessfulLoginBarChart/>
    <Top5AvgTimeSpentPerPageBarChart/>
    <Top5MostUsedFeatureBarChart/>
    <NumOfLoggedInUsersLineChart/>
    <AvgSessionDurationLineChart/>
  </div>
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