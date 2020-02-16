import React from 'react';
import { Link } from 'react-router-dom';
import Container from '@material-ui/core/Container';

import logo from '../../assets/svg/logo.svg';
import styles from './style.module.scss';
import Navigation from '../Navigation';

import { siteData } from '../../api/site';

export default () => {
  return (
    <div className={styles.header}>
      <Container>
        <div className={styles.wrapper}>
          <Link to="/">
            <img alt="CollAction" className={styles.logo} src={logo}></img>
          </Link>
          <Navigation items={siteData.menuLinks.filter((item: any) => !!item.showInPrimaryNavigation)} />
        </div>
      </Container>
    </div>
  );
};
