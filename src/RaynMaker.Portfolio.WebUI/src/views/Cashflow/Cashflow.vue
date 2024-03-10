<template>
  <div>
    <CCard>
      <CCardHeader>
        <CCardTitle>Cashflow</CCardTitle>
      </CCardHeader>
      <div class="card-body">
        <label>Limit: </label>
        <input label="Limit" v-model="limit" @keyup.enter="onLimitChanged" />

        <table class="table-hover">
          <thead>
            <tr>
              <th class="date">Date</th>
              <th class="comment">Type/Comment</th>
              <th class="value">Value</th>
              <th class="value">Balance</th>
            </tr>
          </thead>
          <tbody v-if="Array.isArray(transactions) && transactions.length > 0">
            <tr
              v-for="(t, index) in transactions"
              :key="index"
              style="border-top-style: solid; border-top-width: 1px"
            >
              <td class="date">
                {{ t.date }}
              </td>
              <td
                class="comment"
                style="padding-left: 100px; padding-right: 100px"
              >
                <b>{{ t.type }}</b
                ><br />
                {{ t.comment }}
              </td>
              <td class="value">
                {{ t.value }}
              </td>
              <td class="value">
                {{ t.balance }}
              </td>
            </tr>
          </tbody>
          <tbody v-else>
             <tr>
              <td colspan="4">{{transactions}}</td>
             </tr>
          </tbody>
        </table>
      </div>
    </CCard>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import API from '@/api'
const transactions = ref(null)
const limit = ref(25)

const fetchTransactions = async () => {
  try {
    const response = await API.get(`/cashflow?limit=${limit.value}`)
    transactions.value = response.data
  } catch (error) {
    console.error('Error fetching transactions:', error)
  }
}

onMounted(fetchTransactions)

watch(limit, () => {
  fetchTransactions()
})
</script>

<style scoped>
.date {
  text-align: left;
}
.card-body {
  padding: 16px;
}

.value {
  text-align: right;
}

.comment {
  text-align: center;
}

th,
td {
  padding: 15px;
}

td {
  vertical-align: top;
}
</style>
