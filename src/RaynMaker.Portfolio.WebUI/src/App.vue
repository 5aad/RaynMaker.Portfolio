<template>
  <CWrapper>
    <CHeader fixed with-subheader light>
      <CHeaderBrand style="padding:5px">
        RaynMaker
      </CHeaderBrand>
      <CHeaderNav>
        <CNavItem class="nav">
          <router-link to="/Positions" exact :class="{ 'active': isActive(['/', '/Positions']) }">Positions</router-link>
        </CNavItem>
          <CNavItem class="nav">
            <router-link to="/Performance" :class="{ 'active': isActive('/Performance') }">Performance</router-link>
          </CNavItem>
          <CNavItem class="nav">
            <router-link to="/Cashflow" :class="{ 'active': isActive('/Cashflow') }">Cashflow</router-link>
          </CNavItem>
          <CNavItem class="nav">
            <router-link to="/ClosedPositions" :class="{ 'active': isActive('/ClosedPositions') }">History</router-link>
          </CNavItem>
      </CHeaderNav>
    </CHeader>

    <div class="c-body">
      <main class="c-main">
        <CContainer fluid>
          <transition name="fade" mode="out-in">
            <div>
            <router-view :key="$route.path"/>
          </div>
          </transition>
        </CContainer>
      </main>
    </div>

  </CWrapper>
</template>

<script>
  export default {
    name: 'app',
    methods: {
    isActive (routes) {
      return routes.includes(this.$route.path)
    }
  }
  }
</script>

<style lang="scss">
  @import "~@coreui/coreui/scss/coreui";
  @import "../node_modules/bootstrap/scss/bootstrap";
  @import './assets/css/site.css';
</style>
