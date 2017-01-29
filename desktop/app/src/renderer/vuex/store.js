import Vue from 'vue'
import Vuex from 'vuex'

import * as actions from './actions'
import * as getters from './getters'
import modules from './modules'

Vue.use(Vuex)

var store = new Vuex.Store({
  actions,
  getters,
  modules,
  strict: true
})

if (module.hot) {
  // рассматриваем действия и мутации как модули для горячей замены
  module.hot.accept(['./modules/downloaders'], () => {
    // запрашиваем обновлённые модули
    // (нужно указать .default из-за формата вывода Babel 6)
    const newModuleA = require('./modules/downloaders').default
    // заменяем старые действия и мутации новыми
    store.hotUpdate({
      modules: {
        downloaders: newModuleA
      }
    })
  })
}

export default store
