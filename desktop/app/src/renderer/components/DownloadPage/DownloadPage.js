import { mapGetters, mapActions } from 'vuex'

/* @flow */
export default {
  computed: mapGetters({
    progress: 'progress',
    isDeterminate: 'isInstalling'
  }),
  methods: {
    ...mapActions([
      'downloadAndInstall'
    ])
  }
}
