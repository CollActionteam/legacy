import React from 'react';
import Grid from '@material-ui/core/Grid';
import Drop from '../../assets/svg/drop.svg';
import styles from './style.module.scss';

export default () => {
  const stats: any = [];
  return (
    <Grid container className={styles.main}>
      {stats.map((stat: any, index: number) => (
        <Grid key={index} item xs={12} md={4} className={styles.stat}>
          <h2>{stat.name}</h2>
          <img src={Drop} alt={stat.name} />
        </Grid>
      ))}
    </Grid>
  );
};
