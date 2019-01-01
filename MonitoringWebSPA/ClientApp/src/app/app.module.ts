import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import {NgxPaginationModule} from 'ngx-pagination';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { ConflictDisplayComponent } from './conflict-display/conflict-display.component';
import { EventsFilterComponent } from './events-filter/events-filter.component';

import { AzureSignalRService } from './azure-signal-r.service';
import { EventDisplayService } from './event-display.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    ConflictDisplayComponent,
    EventsFilterComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: ConflictDisplayComponent },
    ]),
    NgxPaginationModule
  ],
  providers: [AzureSignalRService, EventDisplayService],
  bootstrap: [AppComponent]
})
export class AppModule { }
