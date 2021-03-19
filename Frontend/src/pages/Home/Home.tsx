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
      <Section center color="green">
      <p>
          Due to the amazing success and traction of one of our crowdactions - <a href="https://slowfashion.global" target="_blank" rel="noopener noreferrer">The Slow Fashion Season</a> - 
          we have decided to focus our limited resources on further growing the Slow Fashion Movement.
        </p>
        <p>
          Therefore, Collaction.org is currently not actively managed. Although we wholeheartedly believe 
          in the concept and mission, we simply don’t have the time to do both at the moment.           
          That’s why we're looking for an organization or team of people that want to take the proven concept of CollAction.org to the next level! 
          Please reach out to <a href="mailto:hello@collaction.org">hello@collaction.org</a> 
          if you're interested and read more on our blog <a href="https://hellocollaction.medium.com/collaction-org-172731a422e6" target="_blank" rel="noopener noreferrer">here</a>.
        </p>        
      </Section>
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