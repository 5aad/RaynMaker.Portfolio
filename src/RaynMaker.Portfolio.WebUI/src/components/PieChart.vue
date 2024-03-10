<template>
  <Pie :data="chartData" :options="chartOptions">Chart couldn't be loaded.</Pie>
</template>
<script>
import { Chart as ChartJS, ArcElement, Tooltip } from 'chart.js'
import { Pie } from 'vue-chartjs'
import * as pl from '../../../../packages/GooglePaletteJs/palette.js'
ChartJS.register(ArcElement, Tooltip)
export default {
  name: 'App',
  props: {
    data: Array,
    labels: Array
  },
  components: {
    Pie
  },
  computed: {
    chartData (props) {
      let backgrounds = []
        if (props.data) {
        backgrounds = pl.palette('tol', Math.min(props.data.length, 12)).map(hex => '#' + hex)
        }
      return {
        labels: props.labels,
        datasets: [
          {
            data: props.data,
            backgroundColor: backgrounds
          }
        ]
      }
    },
    chartOptions () {
      return {
        responsive: false,
        maintainAspectRatio: false
      }
    }
  }
}
</script>
