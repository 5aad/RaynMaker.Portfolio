<template>
  <div>
    <CCard>
      <CCardHeader>
        <CCardTitle>Positions</CCardTitle>
      </CCardHeader>
      <div style="padding: 16px;">
        <form id="filter">
          <label style="margin-right:10px">Filter: </label>
          <input name="query" v-model="filter" />
          <span style="float: right">
            <label>Total investment: </label> {{ summary.totalInvestment }}
            <span style="margin-left: 20px"></span>
            <label>Current Value: </label> {{ summary.currentValue}}
            <span style="margin-left: 20px"></span>
            <label>Total profit: </label> {{ summary.totalProfit }}
          </span>
        </form>

        <positions-grid :data="positions" :filter-key="filter" style="margin-top:10px"></positions-grid>
      </div>
    </CCard>

    <CCard>
      <CCardHeader>
        <CCardTitle>Diversification</CCardTitle>
      </CCardHeader>
      <div style="padding: 16px;">
        <p>
          Current value
        </p>
        <pie-chart :data="diversification.capital" :labels="diversification.labels"/>
      </div>
    </CCard>
  </div>
</template>

<script>
import { defineComponent } from 'vue'
import API from '@/api'
import PieChart from '@/components/PieChart'
import PositionsGrid from './PositionsGrid'
export default defineComponent({
  name: 'Positions',
   data () {
    return {
      positions: [],
      summary: {},
      filter: '',
      diversification: {
        capital: null,
        labels: null
      }
    }
  },
  components: {
    'positions-grid': PositionsGrid,
    'pie-chart': PieChart
  },
  async created () {
    let response = await API.get('/positions')
    this.positions = response.data.positions
    this.summary = {
      totalInvestment: response.data.totalInvestment,
      currentValue: response.data.currentValue,
      totalProfit: response.data.totalProfit
    }
    response = await API.get('/diversification')
    this.diversification.capital = response.data.capital
    this.diversification.labels = response.data.labels
  }
})
</script>

<style>
  label {
    font-weight: bold
  }
</style>
