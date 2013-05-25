begin
  require 'bundler/setup'
  require 'fuburake'
rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end


FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuMVC.SlickGrid.sln'
	}
				 
	sln.assembly_info = {
		:product_name => "FubuMVC.SlickGrid", # that is correct
		:copyright => 'Copyright 2011-2013 Jeremy D. Miller, Josh Arnold, et al. All rights reserved.'
	}
	
	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
	
	sln.assembly_bottle 'FubuMVC.SlickGrid'
end

desc "Runs the Jasmine tests"
task :open_jasmine do
	serenity "jasmine interactive src/serenity.txt -b Firefox"
end

desc "Runs the Jasmine tests"
task :run_jasmine do
	serenity "jasmine run --timeout 15 src/serenity.txt -b Firefox"
end

desc "Runs the Jasmine tests and outputs the results for CI"
task :run_jasmine_ci do
	serenity "jasmine run --verbose --timeout 15 src/serenity.txt -b Firefox"
end

def self.serenity(args)
  serenity = 'src/packages/Serenity/tools/SerenityRunner.exe'
  sh "#{serenity} #{args}"
end
