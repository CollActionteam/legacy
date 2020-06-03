import React from "react";
import {Button, GhostButton} from "../../components/Button/Button";
import Share from "../../components/Share/Share";
import { Section } from "../../components/Section/Section";
import Carousel from "../../components/Carousel/Carousel";
import CrowdactionsList from "../../components/CrowdactionsList/CrowdactionsList";
import Stats from "../../components/Stats/Stats";
import TimeToAct from "../../components/TimeToAct/TimeToAct";
import styles from "./Home.module.scss";
import sfsLogo from "../../assets/sfs-logo-white.png"

import { useTranslation } from 'react-i18next';
import { Helmet } from "react-helmet";
import {Container, Grid} from "@material-ui/core";
import {Link} from "react-router-dom";

const HomePage = () => {
  const { t } = useTranslation();
  return (
    <>
      <Helmet>
        <title>CollAction</title>
        <meta name="description" content="CollAction" />
      </Helmet>
        <div className={styles.projectHighlightBanner}>
            <Container>
                <Grid container justify="center">
                    <Grid xs={8}>
                        <img src={sfsLogo} alt="Slow Fashion Season" />
                        <h1>Join Slow Fashion Season 2020</h1>
                        <p>If 25,000 people commit to making only conscious fashion choices from June 21st to September 21st, we’ll all do it!</p>
                        <Link to="crowdactions/find">Read more</Link>
                        <br style={{clear: "both"}} />
                    </Grid>
                </Grid>
            </Container>
        </div>
      <Carousel title={t('home.carousel.title')} text={t('home.carousel.text')} />
      <Section center color="grey" title={t('home.intro.title')}>
        <p dangerouslySetInnerHTML={{ __html: t('home.intro.text') }} />
        <GhostButton to="/about">Learn more</GhostButton>
      </Section>
      <Section center title={t('home.timeToAct.title')}>
        <TimeToAct />
      </Section>
      <Section center color="grey" title={t('home.stats.title')}>
        <Stats />
        <Share />
      </Section>
      <Section center title={t('home.crowdactions.title')}>
        <CrowdactionsList />
        <div style={{marginTop: 20 }}>
          <GhostButton to="/crowdactions/find">{t('home.crowdactions.button')}</GhostButton>
        </div>
      </Section>
    </>
  )
}

export default HomePage;