import React from 'react';
import { Link } from 'react-router-dom';
import Container from '@material-ui/core/Container';
import { siteData } from '../../api/site';
import logo from '../../assets/svg/logo.svg';

import Navigation from '../Navigation/Navigation';

import styles from './Header.module.scss';

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
