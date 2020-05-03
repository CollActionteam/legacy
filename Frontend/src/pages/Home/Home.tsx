import React from "react";
import { GhostButton } from "../../components/Button/Button";
import Share from "../../components/Share/Share";
import { Section } from "../../components/Section/Section";
import Carousel from "../../components/Carousel/Carousel";
import ProjectsList from "../../components/ProjectsList/ProjectsList";
import Stats from "../../components/Stats/Stats";

import { useTranslation } from 'react-i18next';
import { Helmet } from "react-helmet";
import TimeToAct from "../../components/TimeToAct/TimeToAct";

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
      <Section center title={t('home.projects.title')}>
        <ProjectsList />
        <div style={{marginTop: 20 }}>
          <GhostButton to="/projects/find">{t('home.projects.button')}</GhostButton>
        </div>
      </Section>
    </>
  )
}

export default HomePage;