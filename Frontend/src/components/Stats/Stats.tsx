import React from 'react';
import { useQuery } from '@apollo/react-hooks';
import { gql } from 'apollo-boost';
import { useTranslation } from 'react-i18next';

import Drop from '../../assets/svg/drop.svg';
import Person from '../../assets/svg/person.svg';
import Waves from '../../assets/svg/waves.svg';

import Grid from '@material-ui/core/Grid';
import Loader from '../Loader/Loader';

import styles from './Stats.module.scss';

export default () => {
  const { t } = useTranslation();
  const query = useQuery(GET_STATISTICS);
  const { data, loading, error } = query;

  if (loading) {
    return <Loader />;
  }

  return (
    <Grid container className={styles.main} justify="center">
      <Grid item xs={12} md={3} className={styles.stat}>
        <div className={styles.circle}>
          <img src={Person} alt={t('home.stats.numberUsers')} />
        </div>
        <h2 className={styles.title}>{data.statistics.numberUsers}</h2>
        <span>{t('home.stats.numberUsers')}</span>
      </Grid>
      <Grid item xs={12} md={3} className={styles.stat}>
        <div className={styles.circle}>
          <img src={Waves} alt={t('home.stats.numberProjects')} />
        </div>
        <h2 className={styles.title}>{data.statistics.numberProjects}</h2>
        <span>{t('home.stats.numberProjects')}</span>
      </Grid>
      <Grid item xs={12} md={3} className={styles.stat}>
        <div className={styles.circle}>
          <img src={Drop} alt={t('home.stats.numberActionsTaken')} />
        </div>
        <h2 className={styles.title}>{data.statistics.numberActionsTaken}</h2>
        <span>{t('home.stats.numberActionsTaken')}</span>
      </Grid>
    </Grid>
  );
};

const GET_STATISTICS = gql`
  query {
    statistics {
      numberActionsTaken
      numberProjects
      numberUsers
    }
  }
`;
