import React from "react";
import { Button, GhostButton } from "../../components/Button/Button";
import { Facebook, Twitter, LinkedIn, Email } from "../../components/Share";
import { Section } from "../../components/Section";
import Carousel from "../../components/Carousel/Carousel";
import ProjectsList from "../../components/ProjectsList";
import Stats from "../../components/Stats";

import { useTranslation } from 'react-i18next';
const HomePage = () => {
  const { t } = useTranslation();
  return (
    <React.Fragment>
      <Carousel title={t('home.carousel.title')} text={t('home.carousel.text')} />
      <Section center color="grey" title={t('home.intro.title')}>
        <p dangerouslySetInnerHTML={{ __html: t('home.intro.text') }} />
        <GhostButton to="/about">Learn more</GhostButton>
      </Section>
      <Section center title={t('home.timeToAct.title')}>
        <p>{t('home.timeToAct.text')}</p>
      </Section>
      <Section center color="grey" title={t('home.stats.title')}>
        <Stats />
      </Section>
      <Section center title={t('home.projects.title')}>
        <ProjectsList />
        <Button to="/projects/find">{t('home.projects.button')}</Button>
      </Section>
      <Section center color="grey">
      <div>
          <h2>{t('home.share.title')}</h2>
          <ul>
            <li>
              <Facebook url="https://www.collaction.org" />
            </li>
            <li>
              <Twitter url="https://www.collaction.org" />
            </li>
            <li>
              <LinkedIn url="https://www.collaction.org" />
            </li>
            <li>
              <Email subject="CollAction" />
            </li>
          </ul>
        </div>
      </Section>
    </React.Fragment>
  )
}

export default HomePage;