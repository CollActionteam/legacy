import React from "react";
import { GhostButton } from "../../components/Button/Button";
import Share from "../../components/Share/Share";
import { Section } from "../../components/Section/Section";
import Carousel from "../../components/Carousel/Carousel";
import CrowdactionsList from "../../components/CrowdactionsList/CrowdactionsList";
import Stats from "../../components/Stats/Stats";
import TimeToAct from "../../components/TimeToAct/TimeToAct";

import { useTranslation } from 'react-i18next';
import { Helmet } from "react-helmet";

const HomePage = () => {
  const { t } = useTranslation();
  return (
    <>
      <Helmet>
        <title>CollAction</title>
        <meta name="description" content="CollAction" />
      </Helmet>
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